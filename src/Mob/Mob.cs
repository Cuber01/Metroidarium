using Godot;

namespace Metroidarium;

public partial class Mob : CharacterBody2D
{
    
    
    public virtual void die()
    {
        CallDeferred("queue_free");
    }
    
    public void getHurt()
    {
        die();
    }
}