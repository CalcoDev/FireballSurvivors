using System.Collections.Generic;
using Game.Autoloads;
using Game.UI.Physical;
using Godot;
using GodotUtilities;

namespace Game.Components;

public partial class HurtboxComponent : Area2D
{
    [Export] public HealthComponent HealthComponent;
    [Export] public FactionComponent FactionComponent;

    [Export]
    public bool TakeContinuousDamage
    {
        get { return _takeContinuousDamage; }
        set
        {
            _takeContinuousDamage = value;

            if (_invincibilityTimer?.IsInsideTree() == false)
                return;

            if (_takeContinuousDamage)
                _invincibilityTimer.Start();
            else
                _invincibilityTimer.Stop();
        }
    }
    [Export] public float InvincibilityTime { get; set; } = 0f;

    [Node] private Timer _invincibilityTimer;

    [Signal]
    public delegate void OnHitEventHandler(HitboxComponent other);

    private readonly List<HitboxComponent> _hitboxes = new();
    private bool _takeContinuousDamage = false;

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
            this.WireNodes();
    }

    public override void _Ready()
    {
        _invincibilityTimer.WaitTime = Mathf.Max(InvincibilityTime, 0.00001f);
        _invincibilityTimer.Connect(Timer.SignalName.Timeout,
            new Callable(this, MethodName.OnInvincibilityTimerTimeout));

        if (TakeContinuousDamage)
            _invincibilityTimer.Start();

        Connect(Area2D.SignalName.AreaEntered, new Callable(this, MethodName.OnAreaEntered));
        Connect(Area2D.SignalName.AreaExited, new Callable(this, MethodName.OnAreaExited));
    }

    private void CheckHitboxes()
    {
        // TODO(calco): This is probably not working. Just in case.
        foreach (HitboxComponent hitbox in _hitboxes)
        {
            // TODO(calco); is this or or and
            if (TakeContinuousDamage && hitbox.ContinuousDamage)
                TryToHit(hitbox);
        }
    }

    private void TryToHit(HitboxComponent hitbox)
    {
        // Want to do if:
        // Ignore invincibility OR
        // Not ignoring invincibility AND invincibility timer is finished OR
        // Not ignoring invincibility AND invincibility time is 0

        bool a = hitbox.IgnoreInvincibility;
        bool b = !hitbox.IgnoreInvincibility && _invincibilityTimer.TimeLeft <= 0f;
        bool c = !hitbox.IgnoreInvincibility && InvincibilityTime <= 0f;

        if (!a && !b && !c)
            return;

        bool sameFaction = FactionComponent.FactionType == hitbox.FactionComponent.FactionType;
        if (FactionComponent == null || sameFaction)
            return;

        EmitSignal(SignalName.OnHit, hitbox);
        hitbox.EmitSignal(HitboxComponent.SignalName.OnHit, this, hitbox);
        HealthComponent?.TakeDamage(hitbox.Damage);

        var p = new FloatingText.FloatingTextParams()
        {
            Text = hitbox.Damage.ToString(),
            Position = GlobalPosition,
            Offset = Vector2.Up * 32f,
            Duration = .3f,
            HoverDuration = 0.05f,
            ShrinkDuration = 0.25f
        };
        GameDirector.Instance.SpawnFloatingText(p);

        _invincibilityTimer.Start();
    }

    private void OnInvincibilityTimerTimeout()
    {
        _invincibilityTimer.Stop();

        CheckHitboxes();
    }

    private void OnAreaEntered(Node body)
    {
        HitboxComponent hitbox = body.GetFirstNodeOfType<HitboxComponent>();

        if (hitbox == null || _hitboxes.Contains(hitbox))
            return;

        _hitboxes.Add(hitbox);
        TryToHit(hitbox);
    }

    private void OnAreaExited(Node body)
    {
        HitboxComponent hitbox = body.GetFirstNodeOfType<HitboxComponent>();

        if (hitbox == null || !_hitboxes.Contains(hitbox))
            return;

        _hitboxes.Remove(hitbox);
    }
}