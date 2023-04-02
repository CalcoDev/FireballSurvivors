using Game.Autoloads;
using Game.Components;
using Game.Components.Abilities;
using Game.Directors.Upgrades;
using Godot;
using GodotUtilities;

namespace Game.GameObjects;

public partial class Player : Actor
{
    public static Player Instance { get; private set; }

    private Vector2 _input;
    private Vector2 _lastNonZeroInput;

    [Node] private HealthComponent _healthComponent;
    [Node] private HurtboxComponent _hurtboxComponent;

    [Node("%Visuals")] private Node2D _visuals;
    [Node] private AnimationPlayer _animationPlayer;

    // TODO(calco): Maybe add this to each actor / enemy??
    [Node("%HealthBar")] private ProgressBar _healthBar;

    #region Lifecycle

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
        {
            if (Instance != null)
                GD.Print("Player already exists! Overwriting...");

            Instance = this;

            this.WireNodes();
        }
    }
    public override void _Ready()
    {
        base._Ready();

        _hurtboxComponent.Connect(HurtboxComponent.SignalName.OnHit,
            new Callable(this, MethodName.OnHit));

        _healthComponent.Connect(HealthComponent.SignalName.OnHealthChanged,
            new Callable(this, MethodName.OnHealthChanged));

        _healthComponent.Connect(HealthComponent.SignalName.OnDied,
            new Callable(this, MethodName.OnDied));

        UpdateHealthDisplay();
    }

    public override void _Process(double delta)
    {
        GatherInput();

        var speed = _input.Normalized();
        AccelerateTowards(speed);
        Move();

        HandleAnimations();
    }

    #endregion

    private void HandleAnimations()
    {
        if (_speed.LengthSquared() > 0)
            _animationPlayer.Play("walk");
        else
            _animationPlayer.Play("RESET");

        _visuals.Scale = new Vector2(_lastNonZeroInput.X > 0f ? 1f : -1f, 1f);
    }

    private void GatherInput()
    {
        _input.X = Input.GetAxis("move_left", "move_right");
        _input.Y = Input.GetAxis("move_up", "move_down");

        if (_input.LengthSquared() > 0f)
            _lastNonZeroInput = _input;
    }

    private void UpdateHealthDisplay()
    {
        _healthBar.Value = _healthComponent.GetHealthPercentage();
    }

    #region Events

    private void OnHit(HitboxComponent hitboxComponent)
    {
        // GD.Print("Player was hit by ", hitboxComponent.Owner.Name);
        GameEvents.EmitPlayerDamaged(hitboxComponent.Damage);
    }

    private void OnHealthChanged(float newHealth, float oldHealth, float maxHealth)
    {
        UpdateHealthDisplay();
    }

    private void OnDied()
    {
        GameEvents.EmitPlayerDied();

        QueueFree();
    }

    #endregion
}
