using Godot;
using static System.Single;
// ReSharper disable InconsistentNaming

namespace Metroidarium;

public partial class SnakeTail : SnakeBody
{
    private readonly PackedScene tailPart = GD.Load<PackedScene>("res://assets/scenes/SnakeTail.tscn");
    

    private int amountOfPartsBehind = -1;

    
    private const float distanceToNextPart = 14f;
    private float deaccelerationSpeed = 0.1f;
    
    public void Init(SnakeBody aheadMe)
    {
        healthComponent = new HealthComponent(this,1);
        Speed = 150f;
        this.aheadMe = aheadMe;
    }
	
    public override void _PhysicsProcess(double delta)
    {
        Vector2 target = constrainDistance(Position, aheadMe.Position, distanceToNextPart);
        if (Position.DistanceTo(aheadMe.Position) > distanceToNextPart)
        {
            Vector2 dir = (target - Position).Normalized();
            Velocity = dir * Speed;
            MoveAndSlide();
        }
    }

    private Vector2 constrainDistance(Vector2 point, Vector2 anchor, float distance) {
        return ((point - anchor).Normalized() * distance) + anchor;
    }

    private Vector2 followVector(Vector2 target)
    {
        var x = target.X - Position.X;
        var y = target.Y - Position.Y;
        return new Vector2(x, y).Normalized() * Speed;
    }

    private void _onHurtboxBodyEntered(Node2D body)
    {
        
    }
}