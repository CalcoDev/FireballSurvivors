using System;
using Godot;
using GodotUtilities;

namespace Game.UI.Screens;

public partial class VictoryScreenUI : CanvasLayer
{
    [Node("%RestartButton")] private Button _restartButton;
    [Node("%QuitButton")] private Button _quitButton;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
            this.WireNodes();
    }

    public override void _Ready()
    {
        GetTree().Paused = true;
        ProcessMode = ProcessModeEnum.Always;

        _restartButton.Connect(Button.SignalName.Pressed,
            new Callable(this, MethodName.OnRestartButtonPressed));

        _quitButton.Connect(Button.SignalName.Pressed,
            new Callable(this, MethodName.OnQuitButtonPressed));
    }

    private void OnRestartButtonPressed()
    {
        GetTree().ChangeSceneToFile("res://scenes/Main.tscn");
        GetTree().Paused = false;
    }

    private void OnQuitButtonPressed()
    {
        GetTree().Quit();
    }
}
