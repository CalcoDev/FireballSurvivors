using Game.Autoloads;
using Game.Directors.Upgrades;
using Game.Resources;
using Godot;
using Godot.Collections;
using GodotUtilities;

namespace Game.Components.Abilities;

public abstract partial class AbilityController : Node
{
    [Export] protected PackedScene AbilityPrefab;
    [Export] protected float BaseCooldown { get; set; } = 1.5f;

    [Node] protected Timer _cooldownTimer;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
            this.WireNodes();
    }

    private Callable _callableCache;
    public override void _Ready()
    {
        _cooldownTimer.Autostart = false;
        _cooldownTimer.WaitTime = BaseCooldown;
        _cooldownTimer.Connect(Timer.SignalName.Timeout, new Callable(this, MethodName.OnCooldownTimerTimeout));
        _cooldownTimer.Start();

        _callableCache = new Callable(this, MethodName.OnUpgradePurchased);
        GameEvents.Instance.Connect(GameEvents.SignalName.OnUpgradePurchased,
            _callableCache);
    }

    public void UnsubscribeFromEvents()
    {
        GameEvents.Instance.Disconnect(GameEvents.SignalName.OnUpgradePurchased, _callableCache);
    }

    protected abstract void ResetToBaseValues();

    protected abstract void ActivateAbility();

    protected virtual void OnUpgradePurchased(UpgradeDirector director,
        AbilityUpgrade upgrade, Dictionary<string, UpgradeData> currentUpgrades)
    {
        ResetToBaseValues();

        foreach (var up in currentUpgrades)
            ComputeUpgradeChange(up.Value.Resource, up.Value.Quantity);
    }

    protected virtual void ComputeUpgradeChange(AbilityUpgrade upgrade, int quantity)
    {
    }

    protected virtual void OnCooldownTimerTimeout()
    {
        _cooldownTimer.WaitTime = BaseCooldown;
        _cooldownTimer.Start();

        ActivateAbility();
    }
}