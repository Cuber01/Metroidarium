using Godot;

namespace Metroidarium;

public partial class Mob : Entity
{
    public float Speed;
    public bool CanFall = true;
    public bool IsOnPlatform = false;
    
    public Vector2 ExternalVelocity;

    // Call after adding internal velocity and before MoveAndSlide
    protected void HandleExternalVelocity()
    {
        Velocity += ExternalVelocity;
        ExternalVelocity = Vector2.Zero;
    }
    
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