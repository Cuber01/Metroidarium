using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Metroidarium;

public partial class Enemy : Mob
{
    [Export] public Mob Target { get; set;}
    protected int MaxHealth = 3;
    protected float Speed = 50f;
    
    public override void _Ready()
    {
        AddComponent(new HealthComponent(this, MaxHealth));
    }
    
    public void setStats(int maxHealth, float speed)
    {
        this.MaxHealth = maxHealth;
        this.Speed = speed;
    }

    private void _onHurtboxBodyEntered(PhysicsBody2D body)
    {
        if (body.IsInGroup("Team Player"))
        {
            //getHurt((Mob)body);
        }
    }
    


}