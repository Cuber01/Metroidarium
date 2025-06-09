using Godot;

namespace Metroidarium;

public partial class Entity : CharacterBody2D
{
    public void getHurt()
    {
        GD.Print("OUCH OUCHIE OUCH STOP STOP AAAAAAAA");
    }
    
    public Vector2 velocity;
}