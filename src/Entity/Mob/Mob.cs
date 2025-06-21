using Godot;

namespace Metroidarium;

public partial class Mob : Entity
{
    public float Speed;
    
    public virtual void die()
    {
        CallDeferred("queue_free");
    }
    
    public virtual void getHurt(int damage)
    {
        GetComponent<HealthComponent>().changeHealth(-damage);
    }
}