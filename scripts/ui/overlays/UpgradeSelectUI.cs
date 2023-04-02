using System;
using System.Collections.Generic;
using Game.Resources;
using Game.UI.Components;
using Godot;
using GodotUtilities;

namespace Game.UI.Overlays;

public partial class UpgradeSelectUI : CanvasLayer
{
    [Export] private PackedScene _abilityUpgradeCard;

    [Node("%CardContainer")] private HBoxContainer _cardContainer;
    [Node] private AnimationPlayer _animationPlayer;

    [Signal]
    public delegate void OnUpgradeSelectedEventHandler(AbilityUpgrade upgrade);

    private readonly List<AbilityUpgradeCardUI> _abilityUpgradeCards = new();

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
            this.WireNodes();
    }

    public override void _Ready()
    {
        GetTree().Paused = true;
        ProcessMode = ProcessModeEnum.Always;

        _animationPlayer.Play("in");
    }

    public override void _ExitTree()
    {
        GetTree().Paused = false;
    }

    public void SetAbiltiyUpgrades(IEnumerable<AbilityUpgrade> upgrades)
    {
        _cardContainer.QueueFreeChildren();
        _abilityUpgradeCards.Clear();

        int i = 0;
        foreach (var upgrade in upgrades)
        {
            var card = _abilityUpgradeCard.Instantiate<AbilityUpgradeCardUI>();
            card.SetData(upgrade);

            _abilityUpgradeCards.Add(card);
            _cardContainer.AddChild(card);

            card.Connect(AbilityUpgradeCardUI.SignalName.OnUpgradeSelected,
                new Callable(this, MethodName._OnUpgradeSelected));

            card.PlayInAnimation(i * 0.25f);

            i += 1;
        }
    }

    private async void _OnUpgradeSelected(AbilityUpgrade upgrade)
    {
        EmitSignal(SignalName.OnUpgradeSelected, upgrade);

        foreach (var card in _abilityUpgradeCards)
        {
            card.PlayOutAnimation(card.Upgrade.Id == upgrade.Id);
        }

        _animationPlayer.Play("out");
        await ToSignal(GetTree().CreateTimer(0.4f), Timer.SignalName.Timeout);

        GetTree().Paused = false;
        QueueFree();
    }
}
