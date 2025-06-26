using Godot;
using System;
using Metroidarium;

public partial class PixelCamera : Camera2D
{
    
    [Export] public Node2D Target;
    private readonly Vector2 offset = new(640/5, 360/5);
    private Vector2 realCameraPosition;
    private ShaderMaterial shader;
    
    public override void _Ready()
    {
        realCameraPosition = GlobalPosition;
        var viewport = GetNode<SubViewportContainer>("../../../../SubViewportContainer");
        shader = (ShaderMaterial)viewport.Material; 
    }
    
    public override void _Process(double delta)
    {
        float dt = (float) delta;

        realCameraPosition.X = Mathf.Lerp(realCameraPosition.X, Target.GlobalPosition.X + offset.X, 5 * dt);
        realCameraPosition.Y = Mathf.Lerp(realCameraPosition.Y, Target.GlobalPosition.Y + offset.Y, 5 * dt);

        Vector2 camSubpixelPos = realCameraPosition.Round() - realCameraPosition;
        shader.SetShaderParameter("cam_offset", camSubpixelPos);

        GlobalPosition = realCameraPosition.Round();
        Align();
    }
}