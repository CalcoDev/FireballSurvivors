using Godot;

namespace Game.Components;

public partial class FactionComponent : Node
{
    public enum Faction : uint
    {
        Player = 0,
        Enemy = 1
    }

    [Export] public Faction FactionType { get; set; } = Faction.Enemy;

    public static bool IsSameFaction(FactionComponent a, FactionComponent b)
    {
        return a.FactionType == b.FactionType;
    }
}