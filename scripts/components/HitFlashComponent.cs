using System;
using Godot;

namespace Game.Components;

public partial class HitFlashComponent : Node
{
    [Export] private HealthComponent _healthComponent;
    [Export] private Sprite2D _sprite;

    [ExportSubgroup("Flash Parameters")]
    [Export] private Color _flashColor = Colors.White;

    [Export] private ShaderMaterial ShaderMaterial;

    private Tween _tween;

    public override void _Ready()
    {
        _sprite.Material = ShaderMaterial;
        _healthComponent.Connect(HealthComponent.SignalName.OnHealthChanged, new Callable(this, MethodName.OnHealthChanged));
    }

    private void OnHealthChanged(float previous, float current, float maximum)
    {
        if (previous <= current)
            return;

        if (_tween?.IsValid() == true)
            _tween.Kill();

        var shaderMat = (ShaderMaterial)_sprite.Material;
        shaderMat.SetShaderParameter("flash_color", _flashColor);
        shaderMat.SetShaderParameter("flash_weight", 1f);

        _tween = GetTree().CreateTween();
        _tween.SetEase(Tween.EaseType.In).SetTrans(Tween.TransitionType.Circ);
        _tween.TweenProperty(_sprite.Material, "shader_parameter/flash_weight", 0f, 0.25f);
        _tween.Play();
    }
}
