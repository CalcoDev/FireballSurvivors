using Game.UI.Physical;
using Godot;

namespace Game.Autoloads;

public partial class GameDirector : Node
{
    public static GameDirector Instance { get; private set; }

    // TODO(calco): Move this into own director to properly pool things etc etc
    [Export] private PackedScene _floatingTextScene;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            if (Instance != null)
                GD.Print("GameDirector already exists. Overwriting...");

            Instance = this;
        }
    }

    public void SpawnFloatingText(FloatingText.FloatingTextParams @params)
    {
        var foreground = GetTree().GetFirstNodeInGroup("foreground_layer");
        if (foreground?.IsInsideTree() == false)
            return;

        var floatingText = _floatingTextScene.Instantiate<FloatingText>();
        foreground.AddChild(floatingText);

        floatingText.Start(@params);
    }
}