using Game.Components;
using Godot;
using GodotUtilities;

namespace Game.GameObjects.Abilities;

public partial class AxeAbility : Node2D
{
    public float Range { get; set; } = 100f;
    public float RotationCount { get; set; } = 2f;
    public float RotationDuration { get; set; } = 1.5f;

    public Vector2 BaseRotationDir;

    public override void _Ready()
    {
        if (BaseRotationDir == Vector2.Zero)
            BaseRotationDir = Vector2.Right.Rotated((float)GD.RandRange(0f, Mathf.Tau));

        var tween = this.CreateTween();
        tween.TweenMethod(new Callable(this, MethodName.TweenMethod), 0f, RotationCount, RotationCount * RotationDuration);
        tween.Connect(Tween.SignalName.Finished, new Callable(this, MethodName.OnTweenCompleted));

        tween.Play();
    }

    public virtual void SetDamage(float damage)
    {
        var hitbox = this.GetFirstNodeOfType<HitboxComponent>();
        if (hitbox != null)
            hitbox.Damage = damage;
    }

    private void TweenMethod(float currentRotation)
    {
        if (Player.Instance?.IsInsideTree() == false)
            return;

        float currentRadius = Range * currentRotation / RotationCount;
        Vector2 currentDir = BaseRotationDir.Rotated(currentRotation * Mathf.Tau);

        GlobalPosition = Player.Instance.GlobalPosition + (currentDir * currentRadius);
    }

    private void OnTweenCompleted()
    {
        QueueFree();
    }
}