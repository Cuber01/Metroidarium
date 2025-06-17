using System;
using Godot;

namespace Metroidarium;

public partial class WalkShootEnemy : Enemy
{
    AStarMoveComponent moveComponent;
    ShootComponent shooter;

    public DateTime StateEnterTime;
    private TimedState<WalkShootEnemy> shootState = new Shoot();
    private TimedState<WalkShootEnemy> runState = new Run();
    private StateMachine<WalkShootEnemy> fsm;
    
    public override void _Ready()
    {
        setStats(3, 50f);
        base._Ready();
        moveComponent = new AStarMoveComponent(this, Speed, Target.Position,
            GetNode<TileMapLayer>("../Level/Floor"),
            GetNode<TileMapLayer>("../Level/Wall"));
        shooter = new ShootComponent(GetParent(), this, "Team Baddies");
        
        fsm = new StateMachine<WalkShootEnemy>(this, runState);
        fsm.AddTransition(runState, shootState, () => runState.TimerCondition());
        fsm.AddTransition(shootState, runState, () => shootState.TimerCondition());
        
    }

    public override void _PhysicsProcess(double delta)
    {
        fsm.Update();
        base._PhysicsProcess(delta);
    }
    
    public class Run : TimedState<WalkShootEnemy>
    {
        public override void Enter(WalkShootEnemy entity)
        {
            base.Enter(entity);
            Delay = 100;
        }
        public override void Update(WalkShootEnemy entity)
        {
            base.Update(entity);
            entity.moveComponent.update(entity.Target.Position);
            entity.MoveAndSlide();
        }
    }
    
    public class Shoot : TimedState<WalkShootEnemy>
    {
        private int shootDelay = 10;
        
        public override void Enter(WalkShootEnemy entity)
        {
            base.Enter(entity);
            Delay = 100;
        }

        public override void Update(WalkShootEnemy entity)
        {
            base.Update(entity);
            if (Time % shootDelay == 0)
            {
                entity.shooter.Shoot(entity.Target.Position, Bullet.EDataType.TargetPosition);
            }
        }
    }
}