using Godot;

namespace Metroidarium;

public partial class Mob : CharacterBody2D
{
    protected HealthComponent HealthComponent;
    
    public virtual void die()
    {
        CallDeferred("queue_free");
    }
    
    public virtual void getHurt(int damage)
    {
        HealthComponent.changeHealth(-damage);
    }
}