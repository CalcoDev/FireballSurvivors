using Game.Components;
using Game.Directors;
using Godot;
using GodotUtilities;

namespace Game.GameObjects.Abilities;

public partial class GaxeAbility : AxeAbility
{
    [Export]
    public float ShootInterval
    {
        get { return shootInterval; }
        set
        {
            shootInterval = value;
            if (_shootTimer != null)
                _shootTimer.WaitTime = value;
        }
    }
    [Export] public float ShootRange { get; set; } = 300f;
    [Export] private PackedScene _projectileScene;

    [Node] private Timer _shootTimer;
    [Node("%FirePoint")] private Node2D _firePoint;

    private float _damage;
    private float shootInterval = 0.5f;

    public override void _Notification(int what)
    {
        base._Notification(what);

        if (what == NotificationSceneInstantiated)
            this.WireNodes();
    }

    public override void _Ready()
    {
        base._Ready();

        _shootTimer.WaitTime = ShootInterval;
        _shootTimer.Connect(Timer.SignalName.Timeout, new Callable(this, MethodName.OnShootTimerTimeout));
    }

    public override void SetDamage(float damage)
    {
        _damage = damage;
    }

    private void OnShootTimerTimeout()
    {
        _shootTimer.WaitTime = ShootInterval + (float)GD.RandRange(-0.1f, 0.1f);
        _shootTimer.Start();

        var dir = (_firePoint.GlobalPosition - GlobalPosition).Normalized().Rotated((float)GD.RandRange(-0.26f, 0.26f));
        var bullet = _projectileScene.Instantiate<Projectile>();
        bullet.GlobalPosition = _firePoint.GlobalPosition;
        bullet.Direction = dir;
        bullet.Damage = _damage;

        GetTree().GetFirstNodeInGroup("foreground_layer")?.AddChild(bullet);
    }
}