using Godot;

namespace Metroidarium;

public partial class ShootingEnemy : Enemy
{
    private float shootDelay = 5;
    private float time = 0;
    
    public override void _Ready()
    {
        setStats(3, 50f);
        base._Ready();
        AddComponent(new ShootComponent(GetParent(), this, "Team Baddies"));
    }

    public override void _PhysicsProcess(double delta)
    {
        time += (float)delta;
        if (time > shootDelay)
        {
            GetComponent<ShootComponent>().Shoot(Target.GlobalPosition, Bullet.EDataType.TargetPosition);
            time = 0;
        }
        base._PhysicsProcess(delta);
    }
}