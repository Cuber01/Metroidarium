using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Metroidarium;

public partial class Enemy : Entity
{
    [Export] public Vector2 TileSize { get; set; }

    [Export] public Entity Target { get; set;}

    AStarMoveComponent moveComponent;
    private float speed = 50f;
    
    public override void _Ready()
    {
        moveComponent = new AStarMoveComponent(this, speed, Target.Position,
            GetNode<TileMapLayer>("../Level/Floor"),
            GetNode<TileMapLayer>("../Level/Wall"));
    }

    public override void _PhysicsProcess(double delta)
    {
        moveComponent.update(Target.Position);
        Velocity = velocity;
        MoveAndSlide();
    }
}