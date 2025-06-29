using System;
using System.Collections.Generic;
using Godot;
using static System.Single;
// ReSharper disable InconsistentNaming

namespace Metroidarium;

public partial class SnakeTail : SnakeBody
{
    private readonly PackedScene tailPart = GD.Load<Godot.PackedScene>("res://assets/scenes/entities/SnakeTail.tscn");
    
    private const float rotationOffset = 1.5f; 
    private int amountOfPartsBehind = -1;
    
    private const float distanceToNextPart = 14f;
    private float deaccelerationSpeed = 0.1f;
    
    public void Init(ref List<SnakeBody> snakeParts, int partId)
    {
        AddComponent(new ContactComponent(1));
        Speed = 150f;
        this.snakeParts = snakeParts;
        this.PartId = partId;
        
        resetPosition();
    }
	
    public override void _PhysicsProcess(double delta)
    {
        GetComponent<HealthComponent>().updateInvincibility((float)delta);
        
        Vector2 target = constrainDistance(Position, AheadMe.Position, distanceToNextPart);
        if (Position.DistanceTo(AheadMe.Position) > distanceToNextPart)
        {
            Vector2 dir = (target - Position).Normalized();
            Velocity = dir * Speed;
            
            SetRotation(Velocity.Angle()+rotationOffset);
            
            MoveAndSlide();
        }
    }

    public void resetPosition()
    {
        GlobalPosition = constrainDistanceWithoutY(Position, AheadMe.Position, distanceToNextPart);
    }

    private Vector2 constrainDistance(Vector2 point, Vector2 anchor, float distance) {
        return ((point - anchor).Normalized() * distance) + anchor;
    }
    
    private Vector2 constrainDistanceWithoutY(Vector2 point, Vector2 anchor, float distance) {
        return ((point - anchor).Normalized() * distance * new Vector2(1,0)) + anchor;
    }
    
    private Vector2 constrainDistanceLikeWhip(Vector2 point, Vector2 anchor, float distance, Vector2 goofiness) {
        return ((point + anchor + goofiness).Normalized() * distance) + anchor;
    }

    private Vector2 followVector(Vector2 target)
    {
        var x = target.X - Position.X;
        var y = target.Y - Position.Y;
        return new Vector2(x, y).Normalized() * Speed;
    }
}