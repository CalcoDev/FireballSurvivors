using System;
using Game.Directors;
using Game.Directors.Upgrades;
using Game.GameObjects;
using Game.Resources;
using Godot;
using Godot.Collections;

namespace Game.Components.Abilities;

public partial class PikeAbilityController : AbilityController
{
    [Export] private float Range { get; set; } = 150f;
    [Export] private float Speed { get; set; } = 100f;
    [Export] private int PierceCount { get; set; } = 1;
    [Export] private float Damage { get; set; } = 30f;
    [Export] public float Knockback { get; set; } = 50f;

    protected override void ActivateAbility()
    {
        if (Player.Instance?.IsInsideTree() == false)
            return;

        var pos = Player.Instance.GlobalPosition;
        var enemies = EnemyDirector.Instance.GetEnemiesInRange(pos, Range);

        if (enemies?.Count == 0)
            return;

        Vector2 dir = (enemies[0].GlobalPosition - pos).Normalized();

        var instance = AbilityPrefab.Instantiate<Projectile>();
        instance.GlobalPosition = pos;
        instance.Speed = Speed;
        instance.Direction = dir;
        instance.PierceCount = PierceCount;
        instance.Damage = Damage;
        instance.Knockback = Knockback;

        GetTree().GetFirstNodeInGroup("foreground_layer").CallDeferred(Node.MethodName.AddChild, instance);
    }

    protected override void ResetToBaseValues()
    {
        Speed = 250f;
        Damage = 15f;
        Knockback = 300f;
        BaseCooldown = 1.5f;
    }

    protected override void ComputeUpgradeChange(AbilityUpgrade upgrade, int quantity)
    {
        if (upgrade.Id == "pike_speed")
        {
            Speed += (quantity * 25f);
        }
        else if (upgrade.Id == "pike_damage")
        {
            Damage += (quantity * 5f);
        }
        else if (upgrade.Id == "pike_knockback")
        {
            Knockback += (quantity * 50f);
        }
        else if (upgrade.Id == "pike_fire_rate")
        {
            BaseCooldown -= (quantity * 0.25f);
        }
    }
}
