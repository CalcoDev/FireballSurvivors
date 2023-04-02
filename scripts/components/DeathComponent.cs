using Godot;
using GodotUtilities;

namespace Game.Components;

public partial class DeathComponent : Node2D
{
    [Export] private Texture2D _texture;
    [Export] private HealthComponent _healthComponent;

    public override void _Ready()
    {
        _healthComponent.Connect(HealthComponent.SignalName.OnDied, new Callable(this, nameof(OnDied)));
    }

    private void OnDied()
    {
        var parent = GetParent<Node2D>();
        var ysort = GetTree().GetFirstNodeInGroup("ysort_layer");

        if (parent == null || ysort == null)
            return;

        parent.RemoveChild(this);
        ysort.AddChild(this);
        GlobalPosition = parent.GlobalPosition;

        this.GetFirstNodeOfType<AnimationPlayer>()?.Play("default");

        var particles = this.GetFirstNodeOfType<GpuParticles2D>();
        if (particles != null)
            particles.Texture = _texture;
    }
}
