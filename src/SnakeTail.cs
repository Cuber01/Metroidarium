using Godot;
using static System.Single;
// ReSharper disable InconsistentNaming

namespace Metroidarium;

public partial class SnakeTail : Entity
{
    private readonly PackedScene tailPart = GD.Load<PackedScene>("res://src/scenes/SnakeBody.tscn");
    
    public void Init(Entity aheadMe, int amountOfPartsBehind, Node2D game)
    {
        this.aheadMe = aheadMe;
        this.amountOfPartsBehind = amountOfPartsBehind;

        if (amountOfPartsBehind > 0)
        {
            behindMe = (SnakeTail)tailPart.Instantiate();
            game.CallDeferred("add_child", behindMe);
            behindMe.Init(this, amountOfPartsBehind - 1, game);
        }
    }

    private int amountOfPartsBehind = -1;
    private Entity aheadMe = null;
    private SnakeTail behindMe = null;
    
    private const float distanceToNextPart = 14f;
    private float speed = 150f;
    private float deaccelerationSpeed = 0.1f;
	
    public override void _PhysicsProcess(double delta)
    {
        Vector2 target = constrainDistance(Position, aheadMe.Position, distanceToNextPart);
        if (Position.DistanceTo(aheadMe.Position) > distanceToNextPart)
        {
            Vector2 dir = (target - Position).Normalized();
            Velocity = dir * speed;
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
        return new Vector2(x, y).Normalized() * speed;
    }
    
    public void changeSpeed(float speed)
    {
        this.speed = speed;
        if (behindMe != null)
        {
            behindMe.changeSpeed(speed);    
        }
    }

    private void _onHurtboxBodyEntered(Node2D body)
    {
        
    }
}