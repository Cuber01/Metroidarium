using Godot;

namespace Metroidarium;

public partial class MovingPlatform : Path2D
{
    [Export] public float Speed = 5f;

    public override void _Ready()
    {
        GetNode<AnimationPlayer>("AnimationPlayer").Play("move");
    }

    private void _onMobEntered(PhysicsBody2D body)
    {
        Mob mob = (Mob)body;
        mob.CanFall = false;
    }
    
    private void _onMobExited(PhysicsBody2D body)
    {
        Mob mob = (Mob)body;
        mob.CanFall = true;
    }
}