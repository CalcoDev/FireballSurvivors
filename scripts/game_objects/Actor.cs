using System;
using Godot;

namespace Game.GameObjects;

public abstract partial class Actor : CharacterBody2D
{
    [ExportSubgroup("Movement")]
    [Export] public float MoveSpeed { get; set; } = 100f;
    [Export] public float Acceleration { get; set; } = 850f;

    protected Vector2 _speed;
    protected Vector2 _knockback;

    public float MovementSpMult { get; set; } = 1f;
    public float AccelerationMult { get; set; } = 1f;

    public void AccelerateTowards(Vector2 dir)
    {
        var maxSpeed = MoveSpeed * MovementSpMult;
        var accel = Acceleration * AccelerationMult * (float)GetProcessDeltaTime();

        _speed.X = Mathf.MoveToward(_speed.X, dir.X * maxSpeed, accel);
        _speed.Y = Mathf.MoveToward(_speed.Y, dir.Y * maxSpeed, accel);
    }

    public void SetVelocity(Vector2 velocity)
    {
        _speed = velocity;
    }

    public void Move()
    {
        Velocity = _speed + _knockback;
        _knockback = Vector2.Zero;

        MoveAndSlide();
        _speed = Velocity;
    }

    public void ApplyKnockback(Vector2 dir, float force)
    {
        _knockback = dir * force;
    }
}