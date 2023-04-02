using Game.GameObjects.Drops;
using Godot;

namespace Game.Components.Drops;

public partial class DropPickupArea : Area2D
{
    public override void _Ready()
    {
        Connect(Area2D.SignalName.AreaEntered, new Callable(this, MethodName.OnPickupAreaBodyEntered));
    }

    private async void OnPickupAreaBodyEntered(Node2D body)
    {
        if (body.GetParent() is not Drop drop)
            return;

        await drop.Yeet(4f);
    }
}