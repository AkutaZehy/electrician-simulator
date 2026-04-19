using Godot;
using System;

public partial class TitleScreen : CanvasLayer
{
    [Signal] public delegate void StartGameEventHandler();

    private ColorRect _fadeOverlay;
    private bool _canInput = false;
    private bool _isTransitioning = false;

    public override void _Ready()
    {
        _fadeOverlay = GetNode<ColorRect>("FadeOverlay");
        _fadeOverlay.Modulate = new Color(1, 1, 1, 1);
        
        Tween t = CreateTween();
        t.TweenProperty(_fadeOverlay, "modulate:a", 0.0f, 1.5f); 
        t.Finished += () => _canInput = true;
    }

    public override void _Input(InputEvent @event)
    {
        if (!_canInput || _isTransitioning) return;

        if (@event is InputEventKey key && key.Pressed || 
            @event is InputEventMouseButton mb && mb.Pressed)
        {
            StartTransition();
        }
    }

    private async void StartTransition()
    {
        _isTransitioning = true;

        Tween t = CreateTween();
        t.TweenProperty(_fadeOverlay, "modulate:a", 1.0f, 1.0f); // 1秒变黑
        
        await ToSignal(t, "finished");

        EmitSignal(SignalName.StartGame);
        
        QueueFree();
    }
}