using Game.Directors.Upgrades;
using Game.GameObjects;
using Game.GameObjects.Abilities;
using Game.Resources;
using Godot;
using Godot.Collections;

namespace Game.Components.Abilities;

public partial class AxeAbilityController : AbilityController
{
    [Export] public float Damage { get; set; } = 30f;
    [Export] public float Range { get; set; } = 75f;

    [Export] public int AxesPerShot { get; set; } = 1;

    protected override void ActivateAbility()
    {
        if (Player.Instance?.IsInsideTree() == false)
            return;

        var spawnPos = Player.Instance.GlobalPosition;

        float angle = Mathf.Tau / AxesPerShot;
        float randomAngle = (float)GD.RandRange(0f, Mathf.Tau);
        for (int i = 0; i < AxesPerShot; i++)
        {
            var instance = AbilityPrefab.Instantiate<AxeAbility>();
            GetTree().GetFirstNodeInGroup("foreground_layer").CallDeferred(Node.MethodName.AddChild, instance);

            instance.GlobalPosition = spawnPos;
            instance.Range = Range;
            instance.BaseRotationDir = Vector2.Right.Rotated(randomAngle + (angle * i));
            instance.SetDamage(Damage);
        }
    }

    protected override void ResetToBaseValues()
    {
        Damage = 30f;
        Range = 75f;
        AxesPerShot = 1;
    }

    protected override void OnUpgradePurchased(UpgradeDirector director,
        AbilityUpgrade upgrade, Dictionary<string, UpgradeData> currentUpgrades)
    {
        if (upgrade.Id == "gaxe")
        {
            var axe = director.GetUpgrade("axe");
            director.RemoveUpgrade(axe, 999);
            director.BlacklistItem(axe);
            director.ApplyUpgrade(director.GetUpgrade("gaxe"), AxesPerShot - 1);

            return;
        }

        base.OnUpgradePurchased(director, upgrade, currentUpgrades);
    }

    protected override void ComputeUpgradeChange(AbilityUpgrade upgrade, int quantity)
    {
        if (upgrade.Id == "axe")
        {
            Damage += (quantity - 1) * 5f;
            AxesPerShot += (int)quantity - 1;
        }
    }
}