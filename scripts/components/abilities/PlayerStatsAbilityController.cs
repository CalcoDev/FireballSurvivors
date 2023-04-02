using Game.Directors.Upgrades;
using Game.GameObjects;
using Game.Resources;
using Godot;
using Godot.Collections;

namespace Game.Components.Abilities;

public partial class PlayerStatsAbilityController : AbilityController
{
    [Export] private float _speedMultiplier = 1f;
    [Export] private float _accelMultiplier = 1f;

    public override void _Ready()
    {
        base._Ready();

        _cooldownTimer.Disconnect(Timer.SignalName.Timeout, new Callable(this, MethodName.OnCooldownTimerTimeout));
        _cooldownTimer.Stop();
    }

    protected override void ActivateAbility()
    {
        if (Player.Instance?.IsInsideTree() == false)
            return;

        Player.Instance.MovementSpMult = _speedMultiplier;
        Player.Instance.AccelerationMult = _accelMultiplier;
    }

    protected override void ResetToBaseValues()
    {
        _speedMultiplier = 1f;
        _accelMultiplier = 1f;
    }

    protected override void OnUpgradePurchased(UpgradeDirector director, AbilityUpgrade upgrade, Dictionary<string, UpgradeData> currentUpgrades)
    {
        base.OnUpgradePurchased(director, upgrade, currentUpgrades);

        ActivateAbility();
    }

    protected override void ComputeUpgradeChange(AbilityUpgrade upgrade, int quantity)
    {
        if (upgrade.Id == "player_speed")
        {
            _speedMultiplier += quantity * 0.15f;
            _accelMultiplier += quantity * 0.15f;
        }
    }
}