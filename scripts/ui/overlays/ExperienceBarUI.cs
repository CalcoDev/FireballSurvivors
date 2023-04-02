using System;
using Game.Directors;
using Godot;
using GodotUtilities;

namespace Game.UI.Overlays;

public partial class ExperienceBarUI : CanvasLayer
{
    [Node("%ProgressBarA")] private ProgressBar _progressBar;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
            this.WireNodes();
    }

    public override void _Ready()
    {
        ExperienceDirector.Instance.Connect(ExperienceDirector.SignalName.OnExperienceUpdated,
            new Callable(this, MethodName.OnExperienceUpdated));
    }

    private void OnExperienceUpdated(int curr, int target)
    {
        _progressBar.Value = (float)((float)curr / target);
    }
}