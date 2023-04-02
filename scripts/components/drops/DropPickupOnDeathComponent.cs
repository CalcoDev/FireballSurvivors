using Game.GameObjects.Drops;
using Game.Logic;
using Godot;

namespace Game.Components.Drops;

public partial class DropPickupOnDeathComponent : Node
{
    [Export] private LootTable _drops;
    [Export(PropertyHint.Range, "0, 1")] private float _dropChance = 0.5f;
    [Export] private HealthComponent _healthComponent;

    public override void _Ready()
    {
        _healthComponent.Connect(HealthComponent.SignalName.OnDied, new Callable(this, MethodName.OnEntityDied));
    }

    private void OnEntityDied()
    {
        float r = GD.Randf();
        if (_drops.Count == 0 || r > _dropChance)
            return;

        var spawnPos = _healthComponent.Owner is Node2D node2d ? node2d.GlobalPosition : Vector2.Zero;
        var dropInstance = _drops.PickRandom<PackedScene>().Instantiate<Drop>();

        // TODO(calco): Probably should cache this somehow.
        GetTree().GetFirstNodeInGroup("ysort_layer").CallDeferred(Node.MethodName.AddChild, dropInstance);

        dropInstance.GlobalPosition = spawnPos;
    }
}