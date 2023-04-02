using Godot;

namespace Game.GameObjects.Eneimes;

public partial class Rat : Enemy
{
    public override void _Process(double delta)
    {
        base._Process(delta);

        var dir = GetDirection();
        AccelerateTowards(dir);
        Move();

        Visuals.Scale = new Vector2(-Mathf.Sign(dir.X), 1);
    }
}