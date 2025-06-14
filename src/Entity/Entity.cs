using Godot;

namespace Metroidarium;

public partial class Entity : CharacterBody2D
{
    
    public virtual void die()
    {
        CallDeferred("queue_free");
    }
    
    public void getHurt()
    {
        die();
    }
    
    public Vector2 velocity;
}