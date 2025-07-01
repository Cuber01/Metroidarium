using System;
using Godot;

namespace Metroidarium;

public partial class WalkShootEnemy : Enemy
{
    private TimedState<WalkShootEnemy> shootState = new Shoot();
    private TimedState<WalkShootEnemy> runState = new Run();
    private StateMachine<WalkShootEnemy> fsm;
    
    public override void _Ready()
    {
        setStats(3, 50f);
        base._Ready();
        AddComponent(new AStarMoveComponent(this, Speed, Target.Position,
            GetNode<TileMapLayer>("../Floor"),
            GetNode<TileMapLayer>("../Wall")));
        AddComponent(new ShootComponent(GetParent(), this, "Team Baddies"));
        
        fsm = new StateMachine<WalkShootEnemy>(this, runState);
        fsm.AddTransition(runState, shootState, () => runState.TimerCondition());
        fsm.AddTransition(shootState, runState, () => shootState.TimerCondition());
        
    }

    public override void _PhysicsProcess(double delta)
    {
        fsm.Update((float)delta);
        base._PhysicsProcess(delta);
    }
    
    public class Run : TimedState<WalkShootEnemy>
    {
        public override void Enter(WalkShootEnemy entity)
        {
            base.Enter(entity);
            Delay = 5f;
        }
        public override void Update(WalkShootEnemy entity, float dt)
        {
            base.Update(entity, dt);
            entity.GetComponent<AStarMoveComponent>().update(entity.Target.Position);
            entity.MoveAndSlide();
        }
    }
    
    public class Shoot : TimedState<WalkShootEnemy>
    {
        private const float ShootDelay = 3f;
        private bool fired = false;

        public override void Enter(WalkShootEnemy entity)
        {
            base.Enter(entity);
            Delay = 5f;
            fired = false;
        }

        public override void Update(WalkShootEnemy entity, float dt)
        {
            base.Update(entity, dt);
            if (Time > ShootDelay && !fired)
            {
                fired = true;
                entity.GetComponent<ShootComponent>().Shoot(entity.Target.Position, Bullet.EDataType.TargetPosition);
            }
        }
    }
}