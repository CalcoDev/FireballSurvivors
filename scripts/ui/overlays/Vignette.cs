using System;
using Game.Autoloads;
using Godot;
using GodotUtilities;

namespace Game.UI.Overlays;

public partial class Vignette : CanvasLayer
{
    [Node] private AnimationPlayer _animationPlayer;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
            this.WireNodes();
    }

    public override void _Ready()
    {
        GameEvents.Instance.Connect(GameEvents.SignalName.OnPlayerDamaged,
            new Callable(this, MethodName.OnPlayerDamaged));

        GameEvents.Instance.Connect(GameEvents.SignalName.OnPlayerDied,
            new Callable(this, MethodName.OnPlayerDied));

        GameEvents.Instance.Connect(GameEvents.SignalName.OnGameStarted,
            new Callable(this, MethodName.OnGameStarted));
    }

    private void OnPlayerDamaged(float damage)
    {
        _animationPlayer.Play("hit");
    }

    private void OnPlayerDied()
    {
        Visible = false;
    }

    private void OnGameStarted()
    {
        Visible = true;
    }
}
