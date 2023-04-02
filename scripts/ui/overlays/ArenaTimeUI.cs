using System;
using Game.Directors;
using Godot;
using GodotUtilities;

namespace Game.UI.Overlays;

public partial class ArenaTimeUI : CanvasLayer
{
    [Export] public ArenaDirector ArenaDirector { get; set; }

    [Node("%TimeLabel")] private Label _timeLabel;



    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
            this.WireNodes();
    }

    public override void _Process(double delta)
    {
        var time = ArenaDirector.GetTimeElapsed();
        _timeLabel.Text = time.ToString("0:00");
    }
}