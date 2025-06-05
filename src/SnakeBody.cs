using Godot;
using static System.Single;
// ReSharper disable InconsistentNaming

namespace Metroidarium;

public partial class SnakeBody : Entity
{

    public void Init(Entity toFollow, int partID)
    {
        this.toFollow = toFollow;
        this.partID = partID;
    }

    private int partID = -1;
    private Entity toFollow = null;
    private float distanceToNextPart = 14f;
    private float speed = 150f;
    private float deaccelerationSpeed = 0.1f;
	
    // Called every frame. 'delta' is the elapsed time since the previous frame.
    public override void _PhysicsProcess(double delta)
    {
        //Vector2 velocity = Velocity;

        // if (Position.DistanceTo(toFollow.Position) > distanceToNextPart)
        // {
        //     velocity.X = Lerp(velocity.X, toFollow.Velocity.X, speed);
        //     velocity.Y = Lerp(velocity.Y, toFollow.Velocity.Y, speed);
        // }

        
        Vector2 target = ConstrainDistance(Position, toFollow.Position, distanceToNextPart);
        if (Position.DistanceTo(toFollow.Position) > distanceToNextPart)
        {
            Vector2 dir = (target - Position).Normalized();
            Velocity = dir * speed;
            MoveAndSlide();
        }
        
        // Vector2 follow = followVector(new_pos);
        //
        // if (follow == Vector2.Zero)
        // {
        //     velocity = Vector2.Zero;
        // }
        // else
        // {
        //     velocity += follow;
        // }
        //
       


    }

    private Vector2 ConstrainDistance(Vector2 point, Vector2 anchor, float distance) {
        return ((point - anchor).Normalized() * distance) + anchor;
    }


    private Vector2 followVector(Vector2 target)
    {
        float x, y, r;
        x = target.X - Position.X;
        y = target.Y - Position.Y;
        return new Vector2(x, y).Normalized() * speed;

    }
}