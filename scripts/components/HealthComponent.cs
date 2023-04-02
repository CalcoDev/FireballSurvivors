using Godot;

namespace Game.Components;

public partial class HealthComponent : Node
{
    [Export] public float MaxHealth { get; set; } = 100;
    [Export] public float Health { get; set; } = 100;

    [Signal]
    public delegate void OnHealthChangedEventHandler(float previous, float current, float maximum);

    [Signal]
    public delegate void OnDiedEventHandler();

    public bool IsDead => Health <= 0;

    public override void _Ready()
    {
        Health = MaxHealth;
    }

    public float GetHealthPercentage()
    {
        if (MaxHealth <= 0)
            return 0;

        return Mathf.Min(Health / MaxHealth, 1f);
    }

    public void TakeDamage(float damage)
    {
        var oldHealth = Health;
        Health -= damage;
        if (Health <= 0)
        {
            Health = 0;
            EmitSignal(SignalName.OnDied);
        }

        EmitSignal(SignalName.OnHealthChanged, oldHealth, Health, MaxHealth);
    }

    public void Heal(float amount)
    {
        var oldHealth = Health;

        Health += amount;
        if (Health > MaxHealth)
            Health = MaxHealth;

        EmitSignal(SignalName.OnHealthChanged, oldHealth, Health, MaxHealth);
    }

    public void ResetHealth()
    {
        var oldHealth = Health;
        Health = MaxHealth;
        EmitSignal(SignalName.OnHealthChanged, oldHealth, Health, MaxHealth);
    }
}