using Godot;

namespace Metroidarium;

public partial class Mob : Entity
{
    public float Speed;
    public bool CanFall = true;

    protected virtual void Fall()
    {
        GetComponent<TweenComponent>().To("scale", Vector2.Zero, 0.3, null, Callable.From( EndFalling ));
    }

    protected virtual void EndFalling()
    {
        die();
    }
    
    public virtual void die()
    {
        CallDeferred("queue_free");
    }
    
    public virtual void getHurt(int damage)
    {
        GetComponent<HealthComponent>().changeHealth(-damage);
    }
}