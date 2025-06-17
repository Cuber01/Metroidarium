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
        AddComponent(new ContactComponent(1));
    }
    
    public void setStats(int maxHealth, float speed)
    {
        this.MaxHealth = maxHealth;
        this.Speed = speed;
    }


    private void _onHurtboxBodyEntered(Node2D body)
    {
        if (body.IsInGroup("Team Player"))
        {
            Entity enemy = (Entity)body;
            getHurt(enemy.GetComponent<ContactComponent>().ContactDamageDealt);
        }
    }
    


}