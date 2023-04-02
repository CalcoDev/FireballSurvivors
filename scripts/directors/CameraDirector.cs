using System;
using Game.Utils;
using Godot;
using GodotUtilities;

namespace Game.Directors;

public partial class CameraDirector : Node2D
{
    public static CameraDirector Instance { get; private set; }

    [Export] public Node2D Target { get; set; }

    [ExportSubgroup("Follow Settings")]
    [Export] public Vector2 CameraOffset { get; set; }
    [Export] public float MouseOffsetMaxRange { get; set; } = 50f;

    [Export(PropertyHint.Range, "0, 1")] public float FollowSpeed { get; set; } = 0.2f;

    private Vector2 _followVel;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            if (Instance != null)
                GD.Print("CameraDirector already exists. Overwriting...");

            Instance = this;
            this.WireNodes();
        }
    }

    public override void _Ready()
    {
        this.GetFirstNodeOfType<Camera2D>().MakeCurrent();
    }

    public override void _Process(double delta)
    {
        if (Target?.IsInsideTree() == false)
            return;

        var d = (float)delta * 4f;
        var mouseOffset = GetMouseOffset();

        var target = Target.GlobalPosition;
        target += mouseOffset + CameraOffset;

        target.X = Calc.SmoothDamp(GlobalPosition.X, target.X, ref _followVel.X, FollowSpeed, 1000f, d);
        target.Y = Calc.SmoothDamp(GlobalPosition.Y, target.Y, ref _followVel.Y, FollowSpeed, 1000f, d);

        GlobalPosition = target;
    }

    private Vector2 GetMouseOffset()
    {
        var mouse = GetViewport().GetMousePosition();

        // Get the size of the window
        var size = GetViewport().GetVisibleRect().Size;
        mouse -= size * 0.5f;
        mouse /= size * 0.5f;

        return mouse * MouseOffsetMaxRange;
    }
}
