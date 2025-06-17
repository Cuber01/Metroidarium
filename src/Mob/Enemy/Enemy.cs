using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Metroidarium;

public partial class Enemy : Mob
{
    [Export] public Mob Target { get; set;}
    
    public override void _Ready()
    {
        HealthComponent = new HealthComponent(this, MaxHealth);
    }

    public override void _PhysicsProcess(double delta)
    {
        checkCollision();
    }

    public void setStats(int maxHealth, float speed)
    {
        this.MaxHealth = maxHealth;
        this.Speed = speed;
    }
    
    private void checkCollision()
    {
        int colCount = GetSlideCollisionCount();
        for(int i = colCount; i > 0; i--)
        {
            Node2D collidingBody = (Node2D)GetSlideCollision(i-1).GetCollider();
            if (collidingBody is SnakeBody enemy)
            {
                enemy.getHurt(1);
                break;
            }
        }
    }
    
    protected int MaxHealth = 3;
    protected float Speed = 50f;

}