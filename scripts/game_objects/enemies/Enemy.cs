using Game.Components;
using Godot;
using GodotUtilities;

namespace Game.GameObjects.Eneimes;

public abstract partial class Enemy : Actor
{
    [Node] protected FactionComponent _factionComponent;
    [Node] protected HurtboxComponent _hurtboxComponent;
    [Node] protected HealthComponent _healthComponent;

    [Node] protected AnimationPlayer _animationPlayer;

    [Node("%Visuals")] protected Node2D Visuals;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
            this.WireNodes();
    }

    public override void _Ready()
    {
        base._Ready();

        _hurtboxComponent.Connect(HurtboxComponent.SignalName.OnHit, new Callable(this, MethodName.OnHit));
        _healthComponent.Connect(HealthComponent.SignalName.OnDied, new Callable(this, MethodName.OnDied));
    }

    protected virtual void OnDied()
    {
        QueueFree();
    }

    protected virtual void OnHit(HitboxComponent other)
    {
        GetNode<RandomAudioStreamPlayer2DComponent>("%HitSFX").PlayRandom();
    }

    protected virtual Vector2 GetDirection()
    {
        if (Player.Instance?.IsInsideTree() == false)
            return Vector2.Zero;

        return (Player.Instance.GlobalPosition - GlobalPosition).Normalized();
    }
}