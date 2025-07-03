using System.Collections.Generic;
using Godot;

namespace Metroidarium;

public partial class MovingPlatform : Path2D
{
    [Export] public float Speed = 5f;
 
    private List<Mob> carriedMobs = new List<Mob>();
    private Vector2 previousPosition;
    private AnimatableBody2D animatableBody;

    public override void _Ready()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("move");
        animatableBody = GetNode<AnimatableBody2D>("AnimatableBody2D");
    }

    public override void _Process(double delta)
    {
        Vector2 velocity = (animatableBody.Position - previousPosition) / (float)delta;
        previousPosition = animatableBody.Position;
        
        foreach (Mob mob in carriedMobs)
        {
            mob.ExternalVelocity += velocity;
        }
    }

    private void _onMobEntered(PhysicsBody2D body)
    {
        Mob mob = (Mob)body;
        carriedMobs.Add(mob);
        mob.CanFall = false;
        mob.IsOnPlatform = true;
    }
    
    private void _onMobExited(PhysicsBody2D body)
    {
        Mob mob = (Mob)body;
        carriedMobs.Remove(mob);
        mob.CanFall = true;
        mob.IsOnPlatform = false;
    }
}