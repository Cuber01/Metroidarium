using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Metroidarium;

public partial class Enemy : Mob
{
    [Export] public Mob Target { get; set;}
    
    public override void _Ready()
    {
        healthComponent = new HealthComponent(this, MaxHealth);
    }
    
    public void setStats(int maxHealth, float speed)
    {
        this.MaxHealth = maxHealth;
        this.Speed = speed;
    }
    
    protected int MaxHealth = 3;
    protected float Speed = 50f;

}