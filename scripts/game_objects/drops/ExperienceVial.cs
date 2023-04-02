using Game.Autoloads;
using Game.UI.Physical;
using Godot;

namespace Game.GameObjects.Drops;

public partial class ExperienceVial : Drop
{
    [Export] public int Amount;

    public override void PickUp()
    {
        GameEvents.EmitExperienceVialCollected(Amount);

        var p = new FloatingText.FloatingTextParams()
        {
            Text = $"+{Amount} EXP",
            Position = GlobalPosition,
            Offset = new Vector2(0f, -16f),
            Duration = 0.25f,
            HoverDuration = 0.05f,
            ShrinkDuration = 0.3f,
        };
        GameDirector.Instance.SpawnFloatingText(p);

        QueueFree();
    }
}
