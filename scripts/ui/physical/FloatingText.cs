using System;
using Game.Autoloads;
using Godot;
using GodotUtilities;

namespace Game.UI.Physical;

public partial class FloatingText : Node2D
{
    public class FloatingTextParams
    {
        public string Text { get; set; }
        public Vector2 Position { get; set; }
        public Vector2 Offset { get; set; }
        public float Duration { get; set; }
        public float HoverDuration { get; set; }
        public float ShrinkDuration { get; set; }
    }

    [Signal] public delegate void OnCompleteEventHandler();

    [Node("%Label")] private Label _label;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
            this.WireNodes();
    }

    public void Start(FloatingTextParams @params)
    {
        GlobalPosition = @params.Position;

        var label = GetNode<Label>("Label");
        label.Text = @params.Text;

        var tween = GetTree().CreateTween();
        tween.SetEase(Tween.EaseType.Out)
            .SetTrans(Tween.TransitionType.Cubic);

        // Go up
        tween.TweenProperty(this, "global_position", GlobalPosition + @params.Offset, @params.Duration);
        tween.Parallel().TweenProperty(this, "scale", new Vector2(1f, 1f), @params.Duration)
            .SetEase(Tween.EaseType.In)
            .SetTrans(Tween.TransitionType.Elastic);

        // Hover for a bit
        tween.Chain().TweenInterval(@params.HoverDuration);

        // Shrink

        var finalPosition = GlobalPosition + @params.Offset;
        var up = finalPosition + new Vector2(0f, 8f);
        var down = finalPosition + new Vector2(0f, -8f);

        var stretch = new Vector2(1.5f, 0.3f);
        var squash = new Vector2(0.3f, 1.5f);

        // Squash and go up
        // tween.Chain().TweenProperty(this, "scale", squash, @params.ShrinkDuration * 0.25f);
        // tween.Parallel().TweenProperty(this, "global_position", up, @params.ShrinkDuration * 0.25f);

        // Stretch and go down
        tween.Chain().TweenProperty(this, "scale", stretch, @params.ShrinkDuration * 0.75f)
            .SetEase(Tween.EaseType.InOut)
            .SetTrans(Tween.TransitionType.Expo);
        // tween.Parallel().TweenProperty(this, "position", down, @params.ShrinkDuration * 0.75f);
        tween.Parallel().TweenProperty(_label, "theme_override_colors/font_color", new Color(1f, 1f, 1f), 0f);
        tween.Parallel().TweenProperty(_label, "theme_override_colors/font_outline_color", new Color(0f, 0f, 0f), 0f);

        // Disappear
        tween.Chain().TweenProperty(this, "scale", new Vector2(0f, 0f), @params.ShrinkDuration * 0.25f);

        // Finished
        tween.Chain().TweenCallback(new Callable(this, MethodName.OnTweenComplete));
    }

    private void OnTweenComplete()
    {
        EmitSignal(SignalName.OnComplete);
        QueueFree();
    }
}
