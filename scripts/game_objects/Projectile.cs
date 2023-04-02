using Game.Components;
using Godot;
using GodotUtilities;

namespace Game.GameObjects;

public partial class Projectile : CharacterBody2D
{
    [Export] public float Speed { get; set; } = 100f;
    [Export]
    public float Damage
    {
        get { return damage; }
        set
        {
            damage = value;
            if (_hitboxComponent != null)
                _hitboxComponent.Damage = damage;
        }
    }
    [Export] public int PierceCount { get; set; } = 1;
    [Export] public float Knockback { get; set; } = 0f;

    public Vector2 Direction { get; set; } = Vector2.Zero;

    [Export] public float Lifetime { get; set; } = -1f;
    [Node] private Timer _lifetimeTimer;

    [Node] private HitboxComponent _hitboxComponent;

    private int _currentPierceCount = 0;
    private float damage = 1f;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
            this.WireNodes();
    }

    public override void _Ready()
    {
        if (Lifetime > 0f)
        {
            _lifetimeTimer.WaitTime = Lifetime;
            _lifetimeTimer.Start(Lifetime);
            _lifetimeTimer.Connect(Timer.SignalName.Timeout, new Callable(this, nameof(OnLifetimeTimerTimeout)));
        }

        _hitboxComponent.Connect(HitboxComponent.SignalName.OnHit, new Callable(this, nameof(OnHit)));
    }

    public override void _Process(double delta)
    {
        Velocity = Direction * Speed;
        Rotation = Direction.Angle();
        MoveAndSlide();

        if (GetSlideCollisionCount() > 0)
            OnCollide();
    }

    public void Die()
    {
        QueueFree();
    }

    private void OnHit(HurtboxComponent hurtbox, HitboxComponent hitbox)
    {
        _currentPierceCount++;

        hurtbox.GetParent<Actor>().ApplyKnockback(Direction, Knockback);

        if (_currentPierceCount >= PierceCount)
            Die();
    }

    private void OnCollide()
    {
        Die();
    }

    private void OnLifetimeTimerTimeout()
    {
        Die();
    }
}