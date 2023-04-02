using System;
using Game.Autoloads;
using Godot;
using GodotUtilities;

namespace Game.Directors;

public partial class ExperienceDirector : Node
{
    public static ExperienceDirector Instance { get; private set; }

    public int Experience { get; private set; } = 0;
    public int TargetExperience { get; private set; } = 5;

    public int Level { get; private set; } = 1;

    [Signal]
    public delegate void OnExperienceUpdatedEventHandler(int curr, int target);

    [Signal]
    public delegate void OnLeveledUpEventHandler(int level);

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            if (Instance != null)
                GD.Print("ExperienceDirector already exists. Overwriting...");

            Instance = this;
        }
    }

    public override void _Ready()
    {
        GameEvents.Instance.Connect(GameEvents.SignalName.OnExperienceVialCollected,
            new Callable(this, MethodName.OnExperienceVialCollected));
    }

    public void AddExperience(int amount)
    {
        Experience = Mathf.Clamp(Experience + amount, 0, TargetExperience);
        EmitSignal(SignalName.OnExperienceUpdated, Experience, TargetExperience);

        if (Experience == TargetExperience)
            LevelUp();
    }

    private void LevelUp()
    {
        Level++;
        Experience = 0;

        // TODO(calco): Yah. It's Desmos time.
        TargetExperience = Mathf.RoundToInt(TargetExperience * 1.5f);

        EmitSignal(SignalName.OnExperienceUpdated, Experience, TargetExperience);
        EmitSignal(SignalName.OnLeveledUp, Level);
    }

    private void OnExperienceVialCollected(int amount)
    {
        AddExperience(amount);
    }
}
