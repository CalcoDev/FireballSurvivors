using System.Threading.Tasks;
using Game.Autoloads;
using Game.UI.Physical;
using Godot;
using GodotUtilities;

namespace Game.GameObjects.Drops;

public abstract partial class Drop : Node2D
{
    public abstract void PickUp();

    private bool _yeeting = false;
    public async Task Yeet(float speed)
    {
        if (_yeeting)
            return;

        _yeeting = true;

        if (Player.Instance?.IsInsideTree() == false)
            return;

        while (true)
        {
            Vector2 s = GlobalPosition;
            Vector2 e = Player.Instance.GlobalPosition;

            float t = (float)GetProcessDeltaTime();
            t = 1f - Mathf.Exp(-speed * t);
            t = Mathf.Clamp(t, 0f, 1f);

            Vector2 prev = GlobalPosition;
            GlobalPosition = s.Lerp(e, t);

            GlobalRotation = (float)(prev - GlobalPosition).Angle() - (Mathf.Pi / 2f);

            if (e.DistanceSquaredTo(GlobalPosition) < 20f * 20f)
                break;

            await ToSignal(GetTree(), SceneTree.SignalName.ProcessFrame);
        }

        var sprite = this.GetFirstNodeOfType<Sprite2D>();

        var tween = GetTree().CreateTween();
        tween.SetEase(Tween.EaseType.OutIn);

        tween.Chain().TweenProperty(sprite, "global_rotation", 0f, 0.05f);
        tween.Parallel().TweenProperty(this, "global_position", Player.Instance.GlobalPosition, 0.05f);
        tween.Parallel().TweenProperty(sprite, "scale", new Vector2(3f, 0.8f), 0.1f);

        tween.Chain().TweenProperty(sprite, "scale", new Vector2(0.8f, 3f), 0.1f);
        tween.Parallel().TweenProperty(sprite, "position", new Vector2(0f, 4f), 0.15f);

        tween.Chain().TweenProperty(sprite, "scale", new Vector2(0f, 0f), 0.1f);
        tween.Parallel().TweenProperty(sprite, "position", new Vector2(0f, -4f), 0.1f);

        tween.Play();
        await ToSignal(tween, Tween.SignalName.Finished);

        PickUp();
    }
}