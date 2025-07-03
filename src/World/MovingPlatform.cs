using System.Collections.Generic;
using Godot;

namespace Metroidarium;

public partial class MovingPlatform : Platform
{
    [Export] public float Speed = 5f;
    
    private Vector2 previousPosition;
    private AnimatableBody2D animatableBody;

    public override void _Ready()
    {
        base._Ready();
        GetNode<AnimationPlayer>("Path2D/AnimationPlayer").Play("move");
        animatableBody = GetNode<AnimatableBody2D>("Path2D/AnimatableBody2D");
    }

    public override void _Process(double delta)
    {
        Vector2 velocity = (animatableBody.Position - previousPosition) / (float)delta;
        previousPosition = animatableBody.Position;
        
        foreach (Mob mob in CarriedMobs)
        {
            mob.ExternalVelocity += velocity;
        }
    }
}