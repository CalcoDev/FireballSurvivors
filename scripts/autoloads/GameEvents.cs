using System;
using Game.Directors.Upgrades;
using Game.Resources;
using Godot;
using Godot.Collections;

namespace Game.Autoloads;

public partial class GameEvents : Node
{
    public static GameEvents Instance { get; private set; }

    [Signal]
    public delegate void OnPlayerDiedEventHandler();

    [Signal]
    public delegate void OnExperienceVialCollectedEventHandler(int amount);

    [Signal]
    public delegate void OnUpgradePurchasedEventHandler(UpgradeDirector director, AbilityUpgrade upgrade, Dictionary<string, UpgradeData> currentUpgrades);

    [Signal]
    public delegate void OnUpgradeRemovedEventHandler(UpgradeDirector director, AbilityUpgrade upgrade, Dictionary<string, UpgradeData> currentUpgrades);

    [Signal]
    public delegate void OnPlayerDamagedEventHandler(float damage);

    [Signal]
    public delegate void OnGameStartedEventHandler();

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            if (Instance != null)
                GD.Print("GameEvents already exists. Overwriting...");

            Instance = this;
        }
    }

    public static void EmitExperienceVialCollected(int amount)
    {
        Instance.EmitSignal(SignalName.OnExperienceVialCollected, amount);
    }

    public static void EmitUpgradePurchased(UpgradeDirector director,
        AbilityUpgrade upgrade, Dictionary<string, UpgradeData> currentUpgrades)
    {
        Instance.EmitSignal(SignalName.OnUpgradePurchased, director, upgrade, currentUpgrades);
    }

    public static void EmitUpgradeRemoved(UpgradeDirector director,
        AbilityUpgrade upgrade, Dictionary<string, UpgradeData> currentUpgrades)
    {
        Instance.EmitSignal(SignalName.OnUpgradeRemoved, director, upgrade, currentUpgrades);
    }

    public static void EmitPlayerDamaged(float damage)
    {
        Instance.EmitSignal(SignalName.OnPlayerDamaged, damage);
    }

    public static void EmitPlayerDied()
    {
        Instance.EmitSignal(SignalName.OnPlayerDied);
    }

    public static void EmitGameStarted()
    {
        Instance.EmitSignal(SignalName.OnGameStarted);
    }
}
