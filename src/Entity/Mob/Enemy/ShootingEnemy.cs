using Godot;

namespace Metroidarium;

public partial class ShootingEnemy : Enemy
{
    private int delay = 100;
    private int counter = 0;
    
    public override void _Ready()
    {
        setStats(3, 50f);
        base._Ready();
        AddComponent(new ShootComponent(GetParent(), this, "Team Baddies"));
    }

    public override void _PhysicsProcess(double delta)
    {
        if (counter == 0)
        {
            GetComponent<ShootComponent>().Shoot(Target.GlobalPosition, Bullet.EDataType.TargetPosition);
            counter = delay;
        }
        counter--;
        
        base._PhysicsProcess(delta);
    }
}