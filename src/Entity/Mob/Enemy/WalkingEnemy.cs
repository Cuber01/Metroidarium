using Godot;

namespace Metroidarium;

public partial class WalkingEnemy : Enemy
{
    public override void _Ready()
    {
        setStats(3, 50f);
        base._Ready();
        AddComponent(new AStarMoveComponent(this, Speed, Target.Position,
            GetNode<TileMapLayer>("../Level/Floor"),
            GetNode<TileMapLayer>("../Level/Wall")));
    }

    public override void _PhysicsProcess(double delta)
    {
        GetComponent<AStarMoveComponent>().update(Target.Position);
        MoveAndSlide();
        base._PhysicsProcess(delta);
    }
}