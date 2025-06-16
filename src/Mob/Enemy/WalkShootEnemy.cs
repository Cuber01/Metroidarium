using Godot;

namespace Metroidarium;

public partial class WalkShootEnemy : Enemy
{
    AStarMoveComponent moveComponent;
    ShootComponent shooter;
    
    //private IState state;
    
    public override void _Ready()
    {
        setStats(3, 50f);
        base._Ready();
        moveComponent = new AStarMoveComponent(this, Speed, Target.Position,
            GetNode<TileMapLayer>("../Level/Floor"),
            GetNode<TileMapLayer>("../Level/Wall"));
        shooter = new ShootComponent(GetParent(), this, "Team Baddies");
    }

    public override void _PhysicsProcess(double delta)
    {
        //state.Update();
    }

    public class RunState : IState
    {
        public void Update()
        {
            GD.Print("RunState.Update");
        }
    }
    
    public class WalkState : IState
    {
        public void Update()
        {
            GD.Print("WalkState.Update");
        }
    }
}