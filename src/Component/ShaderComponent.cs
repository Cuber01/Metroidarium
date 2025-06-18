using System;
using Godot;

namespace Metroidarium;

public class ShaderComponent : Component
{
    public ShaderMaterial Shader
    {
        get;
        private set;
    }
    
    public ShaderComponent(Entity actor)
    {
        Sprite2D sprite = (Sprite2D)actor.GetNode("Sprite2D");
        Shader = (ShaderMaterial)sprite.Material;
    }
    
}