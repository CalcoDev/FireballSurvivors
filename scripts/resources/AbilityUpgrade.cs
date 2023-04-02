using Game.Logic;
using Godot;

namespace Game.Resources;

public partial class AbilityUpgrade : Resource
{
    [Export] public string Id;

    [Export] public string Name;
    [Export(PropertyHint.MultilineText)] public string Description;

    [Export] public int MaxLevel = -1;

    [Export] public LootTable AddonLootTable;
}