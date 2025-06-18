using System;
using Godot;

namespace Metroidarium;

public class ShaderComponent : Component
{
    public ShaderMaterial shader
    {
        get;
        private set;
    }
    
    public ShaderComponent(Entity actor)
    {
        Sprite2D sprite = (Sprite2D)actor.GetNode("Sprite2D");
        shader = (ShaderMaterial)sprite.Material;
    }
    
}