using System;
using Game.Resources;
using Godot;
using GodotUtilities;

namespace Game.UI.Components;

public partial class AbilityUpgradeCardUI : Control
{
    [Node("%NameLabel")] private Label _nameLabel;
    [Node("%DescriptionLabel")] private Label _descriptionLabel;

    [Node("ButtonStateAnimator")] private AnimationPlayer _btnStAnimator;
    [Node("InOutAnimator")] private AnimationPlayer _inOutAnimator;

    [Signal]
    public delegate void OnUpgradeSelectedEventHandler();

    public AbilityUpgrade Upgrade { get; private set; }

    public override void _Notification(int what)
    {
        if (what == NotificationSceneInstantiated)
            this.WireNodes();
    }

    public override void _Ready()
    {
        Connect(SignalName.GuiInput, new Callable(this, MethodName.OnInput));

        Connect(SignalName.MouseEntered, new Callable(this, MethodName.OnMouseEntered));
    }

    public async void PlayInAnimation(float delay)
    {
        Modulate = new Color(1f, 1f, 1f, 0f);

        var timer = GetTree().CreateTimer(delay);
        await timer.ToSignal(timer, Timer.SignalName.Timeout);

        _inOutAnimator.Play("in");
    }

    public async void PlayOutAnimation(bool wasSelected)
    {
        if (wasSelected)
            _inOutAnimator.Play("out_selected");
        else
            _inOutAnimator.Play("out_discarded");


        await ToSignal(_inOutAnimator, AnimationPlayer.SignalName.AnimationFinished);
        // QueueFree();
    }

    public void SetData(AbilityUpgrade upgrade)
    {
        _nameLabel.Text = upgrade.Name;
        _descriptionLabel.Text = upgrade.Description;

        Upgrade = upgrade;
    }

    private bool _disabled;
    private void OnInput(InputEvent @event)
    {
        if (@event.IsActionPressed("select_card") && !_disabled)
        {
            _disabled = true;
            EmitSignal(SignalName.OnUpgradeSelected, Upgrade);
        }
    }

    private void OnMouseEntered()
    {
        _btnStAnimator.Play("hover");
    }
}
