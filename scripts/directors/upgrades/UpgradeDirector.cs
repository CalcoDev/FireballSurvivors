using System.Collections.Generic;
using Game.Autoloads;
using Game.Components.Abilities;
using Game.GameObjects;
using Game.Logic;
using Game.Resources;
using Game.UI.Overlays;
using Godot;
using Godot.Collections;

namespace Game.Directors.Upgrades;

public partial class UpgradeDirector : Node
{
    public static UpgradeDirector Instance { get; private set; }

    [Export] private ExperienceDirector _experienceDirector;
    [Export] private PackedScene _upgradeSelectUI;

    [Export] private LootTable _upgradePool;

    private readonly Godot.Collections.Dictionary<string, UpgradeData> _upgrades = new();
    private readonly System.Collections.Generic.Dictionary<AbilityUpgrade, AbilityController> _abilityControllers = new();
    private readonly List<AbilityUpgrade> _blacklistedUpgrades = new();

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            if (Instance != null)
                GD.Print("Upgrade director already exists! Overwriting...");

            Instance = this;
        }
    }

    public override void _Ready()
    {
        _experienceDirector.Connect(ExperienceDirector.SignalName.OnLeveledUp, new Callable(this, MethodName.OnLeveledUp));

        // TODO(calco): Automate this some other way lmao
        var bronzeSword = GD.Load<AbilityUpgrade>("res://resources/upgrades/abilities/BronzeSwordAbility.tres");
        var playerStats = GD.Load<AbilityUpgrade>("res://resources/upgrades/abilities/PlayerStatsAbility.tres");

        AddUpgradeInternal(bronzeSword);
        AddUpgradeInternal(playerStats);
    }

    public void AddUpgradeInternal(AbilityUpgrade upgrade)
    {
        _upgrades.Add(upgrade.Id, new UpgradeData { Resource = upgrade, Quantity = 1 });
        _upgradePool.AddFrom(upgrade.AddonLootTable);
    }

    public void ApplyUpgrade(AbilityUpgrade upgrade, int count)
    {
        if (!_upgrades.ContainsKey(upgrade.Id))
        {
            _upgrades.Add(upgrade.Id, new UpgradeData { Resource = upgrade, Quantity = 0 });

            if (upgrade.AddonLootTable != null)
                _upgradePool.AddFrom(upgrade.AddonLootTable);

            if (upgrade is Ability ability)
            {
                var node = ability.AbilityControllerScene.Instantiate<AbilityController>();
                AddChild(node);
                _abilityControllers.Add(ability, node);
            }
        }

        _upgrades[upgrade.Id].Quantity += count;

        GameEvents.EmitUpgradePurchased(this, upgrade, _upgrades);
    }

    public void RemoveUpgrade(AbilityUpgrade upgrade, int count)
    {
        if (!_upgrades.ContainsKey(upgrade.Id))
            return;

        _upgrades[upgrade.Id].Quantity -= count;
        if (_upgrades[upgrade.Id].Quantity <= 0)
        {
            _upgrades.Remove(upgrade.Id);

            if (upgrade.AddonLootTable != null)
                _upgradePool.RemoveFrom(upgrade.AddonLootTable);

            if (upgrade is Ability ability)
            {
                _abilityControllers[ability].UnsubscribeFromEvents();
                _abilityControllers[ability].QueueFree();
                _abilityControllers.Remove(ability);
            }
        }

        GameEvents.EmitUpgradeRemoved(this, upgrade, _upgrades);
    }

    public void BlacklistItem(AbilityUpgrade upgrade)
    {
        _blacklistedUpgrades.Add(upgrade);
    }

    public AbilityUpgrade GetUpgrade(string id)
    {
        return _upgrades.TryGetValue(id, out var data) ? data.Resource : null;
    }

    public IList<AbilityUpgrade> PickUpgrades()
    {
        return _upgradePool.PickUniqueWithCondition<AbilityUpgrade>(3,
            upgrade => !_blacklistedUpgrades.Contains(upgrade) &&
            (!_upgrades.TryGetValue(upgrade.Id, out var data) || upgrade.MaxLevel == -1 || data.Quantity < upgrade.MaxLevel));
    }

    private void OnLeveledUp(int level)
    {
        var upgradeSelectUI = _upgradeSelectUI.Instantiate<UpgradeSelectUI>();
        AddChild(upgradeSelectUI);

        upgradeSelectUI.SetAbiltiyUpgrades(PickUpgrades());

        upgradeSelectUI.Connect(UpgradeSelectUI.SignalName.OnUpgradeSelected,
            new Callable(this, MethodName.OnUpgradeSelected));
    }

    private void OnUpgradeSelected(AbilityUpgrade upgrade)
    {
        ApplyUpgrade(upgrade, 1);
    }
}
