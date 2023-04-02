using Game.Directors.Upgrades;
using Game.GameObjects;
using Game.GameObjects.Abilities;
using Game.Resources;
using Godot;
using Godot.Collections;

namespace Game.Components.Abilities;

public partial class GaxeAbilityController : AbilityController
{
    [Export] public float Damage { get; set; } = 30f;
    [Export] public float Range { get; set; } = 75f;

    [Export] public int GaxesPerShot { get; set; } = 1;

    private float _fireSpeed = 0.5f;

    protected override void ActivateAbility()
    {
        if (Player.Instance?.IsInsideTree() == false)
            return;

        var spawnPos = Player.Instance.GlobalPosition;

        float angle = Mathf.Tau / GaxesPerShot;
        float randomAngle = (float)GD.RandRange(0f, Mathf.Tau);
        for (int i = 0; i < GaxesPerShot; i++)
        {
            var instance = AbilityPrefab.Instantiate<GaxeAbility>();
            GetTree().GetFirstNodeInGroup("foreground_layer").CallDeferred(Node.MethodName.AddChild, instance);

            instance.GlobalPosition = spawnPos;
            instance.Range = Range;
            instance.BaseRotationDir = Vector2.Right.Rotated(randomAngle + (angle * i));
            instance.ShootInterval = _fireSpeed;
            instance.SetDamage(Damage);
        }
    }

    protected override void ResetToBaseValues()
    {
        GaxesPerShot = 1;
        Damage = 30f;
        Range = 75f;
        _fireSpeed = 0.5f;
    }

    protected override void ComputeUpgradeChange(AbilityUpgrade upgrade, int quantity)
    {
        if (upgrade.Id == "gaxe")
        {
            GaxesPerShot = quantity - 1;
        }
        else if (upgrade.Id == "gaxe_firespeed")
        {
            _fireSpeed -= quantity * 0.1f;
        }
        else if (upgrade.Id == "gaxe_damage")
        {
            Damage += (quantity - 1) * 5f;
        }

        if (upgrade.Id == "gaxe_count")
        {
            GaxesPerShot += quantity - 1;
        }
    }
}