using Game.Autoloads;
using Godot;
using GodotUtilities;

namespace Game.Directors;

public partial class ArenaDirector : Node
{
    public static ArenaDirector Instance { get; private set; }

    [Export] private float DifficultyInterval = 5f;

    [Export] private float ArenaDifficulty;

    [Export] private PackedScene _victoryScreenScene;
    [Export] private PackedScene _defeatScreenScene;

    [Signal]
    public delegate void OnDifficultyChangedEventHandler(float difficulty);

    [Node] private Timer _timer;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            if (Instance != null)
                GD.Print("ArenaDirector already exists. Overwriting...");

            Instance = this;
            this.WireNodes();
        }
    }

    public override void _Ready()
    {
        GameEvents.EmitGameStarted();

        _timer.Connect(Timer.SignalName.Timeout,
            new Callable(this, MethodName.OnTimerTimeout));

        GameEvents.Instance.Connect(GameEvents.SignalName.OnPlayerDied,
            new Callable(this, MethodName.OnPlayerDied));
    }

    public override void _Process(double delta)
    {
        float nextTimeTarget = (float)_timer.WaitTime - ((ArenaDifficulty + 1) * DifficultyInterval);

        if (_timer.TimeLeft <= nextTimeTarget)
        {
            ArenaDifficulty++;
            EmitSignal(SignalName.OnDifficultyChanged, ArenaDifficulty);
        }
    }

    public float GetTimeElapsed()
    {
        return (float)(_timer.WaitTime - _timer.TimeLeft);
    }

    private void OnTimerTimeout()
    {
        GetTree().ChangeSceneToPacked(_victoryScreenScene);
    }

    private void OnPlayerDied()
    {
        GetTree().ChangeSceneToPacked(_defeatScreenScene);
    }
}