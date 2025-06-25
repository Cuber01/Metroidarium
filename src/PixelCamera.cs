using Godot;
using System;
using Metroidarium;

public partial class PixelCamera : Camera2D
{
    [Export] public Node2D Target;

    private readonly Vector2 gameSize = new(640, 360);
    private float windowScale;
    private Vector2 realCameraPosition;

    public override void _Ready()
    {
        windowScale = (GetWindow().Size / gameSize).X;
        realCameraPosition = GlobalPosition;
    }

    public override void _PhysicsProcess(double delta)
    {
        float dt = (float) delta;
        
        // # Use another lerp to make the movement smooth
        realCameraPosition.X = lerpi(realCameraPosition.X, Target.GlobalPosition.X, 5 * dt);
        realCameraPosition.Y = lerpi(realCameraPosition.Y, Target.GlobalPosition.Y, 5 * dt);
        
        // # Calculate the "subpixel" position of the new camera position
        Vector2 camSubpixelPos = realCameraPosition.Round() - realCameraPosition;

        // # Update the Main ViewportContainer's shader uniform
        var viewport = GetNode<SubViewportContainer>("../../../SubViewportContainer");
        var shader = (ShaderMaterial)viewport.Material; 
        shader.SetShaderParameter("cam_offset", camSubpixelPos);

        // # Set the camera's position to the new position and round it.
        GlobalPosition = realCameraPosition.Round(); 
    }

    public float lerpi(float origin, float target, float weight)
    {
        target = Mathf.Floor(target);
        origin = Mathf.Floor(origin);
        float distance = Mathf.Ceil(Mathf.Abs(target - origin) * weight);
        return Mathf.MoveToward(origin, target, distance);
    }
    

}
