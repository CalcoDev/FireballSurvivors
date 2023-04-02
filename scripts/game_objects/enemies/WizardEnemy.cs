using Game.Utils;
using Godot;
using GodotUtilities;

namespace Game.GameObjects.Eneimes;

public partial class WizardEnemy : Enemy
{
    [Export] private PackedScene _projectileScene;

    [Node] private Timer _attackTimer;

    private float _wanderTheta;

    private const float kFullAvoidRangeSqr = 100f * 100f;
    private const float kFullWanderRangeSqr = 175f * 175f;
    private const float kFullSeekRangeSqr = 250f * 250f;

    [Node("%Staff")] private Node2D _staff;
    [Node("%FirePoint")] private Node2D _firePoint;

    public override void _Ready()
    {
        base._Ready();

        _attackTimer.Connect(Timer.SignalName.Timeout, new Callable(this, nameof(OnAttackTimerTimeout)));
    }

    public override void _Process(double delta)
    {
        // Seek
        var targetPos = GlobalPosition;
        if (Player.Instance?.IsInsideTree() == true)
            targetPos = Player.Instance.GlobalPosition;

        var seekDir = (targetPos - GlobalPosition).Normalized();

        // Avoid
        var avoidDir = (GlobalPosition - targetPos).Normalized();

        // Wander
        const float displacement = 0.1f;
        _wanderTheta += (float)GD.RandRange(-displacement, displacement);
        var wanderDir = new Vector2(Mathf.Cos(_wanderTheta * Mathf.Tau), Mathf.Sin(_wanderTheta * Mathf.Tau));

        // Combine
        float d = (GlobalPosition - targetPos).LengthSquared();

        float d0 = Mathf.Clamp(d, kFullAvoidRangeSqr, kFullWanderRangeSqr);
        float d1 = Mathf.Clamp(d, kFullWanderRangeSqr, kFullSeekRangeSqr);

        float w0 = Calc.MapRange(d0, kFullAvoidRangeSqr, kFullWanderRangeSqr, 0f, 1f); // 0 means avoid, 1 means wander
        float w1 = 1f - Calc.MapRange(d1, kFullWanderRangeSqr, kFullSeekRangeSqr, 0f, 1f); // 0 means wander, 1 means seek

        float wAvoid = 1f - w0;
        float wWander = (w0 + w1) / 2f;
        float wSeek = 1f - w1;

        // GD.Print($"d: {d}, d0: {d0}, d1: {d1}");
        // GD.Print($"Avoid: {wAvoid}, Wander: {wWander}, Seek: {wSeek}\n");

        var finalDir = (avoidDir * wAvoid) + (wanderDir * wWander) + (seekDir * wSeek);

        AccelerateTowards(finalDir);
        Move();

        if (_speed.LengthSquared() > 0f)
            _animationPlayer.Play("walk");
        else
            _animationPlayer.Play("RESET");

        Visuals.Scale = new Vector2(Mathf.Sign(seekDir.X), 1);

        float angle = Vector2.Right.Rotated(seekDir.Angle() + (Mathf.Pi / 2f)).Angle();
        if (Visuals.Scale.X < 0f)
            angle = Mathf.Pi - angle + Mathf.Pi;

        _staff.Rotation = angle;
    }

    private void OnAttackTimerTimeout()
    {
        if (Player.Instance?.IsInsideTree() == false)
            return;

        var dir = (Player.Instance.GlobalPosition - _firePoint.GlobalPosition).Normalized();
        var bullet = _projectileScene.Instantiate<Projectile>();
        bullet.GlobalPosition = _firePoint.GlobalPosition;
        bullet.Direction = dir;

        GetTree().GetFirstNodeInGroup("foreground_layer")?.AddChild(bullet);
    }
}
