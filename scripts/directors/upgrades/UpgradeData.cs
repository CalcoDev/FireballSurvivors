using Game.Resources;
using Godot;

namespace Game.Directors.Upgrades;

public partial class UpgradeData : RefCounted
{
    public AbilityUpgrade Resource;
    public int Quantity;
}