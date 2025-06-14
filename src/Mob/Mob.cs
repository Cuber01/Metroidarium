using Godot;

namespace Metroidarium;

public partial class Mob : CharacterBody2D
{
    protected HealthComponent healthComponent;
    
    public virtual void die()
    {
        CallDeferred("queue_free");
    }
    
    public virtual void getHurt(int damage)
    {
        healthComponent.changeHealth(-damage);
    }
}