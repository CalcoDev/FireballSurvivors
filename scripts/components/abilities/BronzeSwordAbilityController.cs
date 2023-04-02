using System.Linq;
using Game.Directors;
using Game.Directors.Upgrades;
using Game.GameObjects;
using Game.Resources;
using Godot;
using Godot.Collections;
using GodotUtilities;

namespace Game.Components.Abilities;

public partial class BronzeSwordAbilityController : AbilityController
{
    [Export] private float Damage { get; set; } = 1f;
    [Export] private float Range { get; set; } = 150f;

    protected override void ActivateAbility()
    {
        var enemiesInRange = EnemyDirector.Instance.GetEnemiesInRange(Player.Instance.GlobalPosition, Range);
        if (enemiesInRange == null || enemiesInRange.Count == 0)
            return;

        var spawnPos = enemiesInRange[0].GlobalPosition;
        var rotation = (Player.Instance.GlobalPosition - spawnPos).Angle();

        var instance = AbilityPrefab.Instantiate<Node2D>();

        GetTree().GetFirstNodeInGroup("foreground_layer").CallDeferred(Node.MethodName.AddChild, instance);

        instance.GlobalPosition = spawnPos + (Vector2.Right.Rotated((float)GD.RandRange(0, Mathf.Tau)) * 10f);
        instance.Rotation = rotation;

        instance.GetFirstNodeOfType<HitboxComponent>().Damage = Damage;
    }

    protected override void ResetToBaseValues()
    {
        Damage = 20f;
        BaseCooldown = 1.5f;
    }

    protected override void ComputeUpgradeChange(AbilityUpgrade upgrade, int quantity)
    {
        if (upgrade.Id == "bronze_sword_attack_rate")
        {
            _cooldownTimer.WaitTime = Mathf.Max(BaseCooldown * (1 - (quantity * 0.1f)), 0.05f);
            _cooldownTimer.Start();
        }

        if (upgrade.Id == "bronze_sword_damage")
        {
            Damage = 20f * (0.5f + ((quantity - 1) * 0.1f));
        }
    }
}