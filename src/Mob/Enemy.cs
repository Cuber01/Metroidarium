using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Metroidarium;

public partial class Enemy : Mob
{
    [Export] public Mob Target { get; set;}
    [Export] public bool TurnedOn { get; set; }

    AStarMoveComponent moveComponent;
    
    private float speed = 50f;
    private int maxHealth = 3;
    
    public override void _Ready()
    {
        healthComponent = new HealthComponent(this, maxHealth);
        
        if (!TurnedOn) return;
        
        moveComponent = new AStarMoveComponent(this, speed, Target.Position,
            GetNode<TileMapLayer>("../Level/Floor"),
            GetNode<TileMapLayer>("../Level/Wall"));
    }

    public override void _PhysicsProcess(double delta)
    {
        if (!TurnedOn) return;
        
        moveComponent.update(Target.Position);
        MoveAndSlide();
    }
    
    private void _onHurtboxBodyEntered(Node2D body)
    {
        
    }
}