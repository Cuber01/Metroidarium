using Godot;
using System;

public partial class Camera2d : Camera2D
{
	[Export] public Node2D toFollow;

	private Vector2 realCamPosition;
	
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		realCamPosition = Position.Lerp(toFollow.Position, (float)(delta*3));
		Vector2 subpixelCamOffset = realCamPosition.Round() - realCamPosition;
		ShaderMaterial shader = (ShaderMaterial)((SubViewportContainer)GetParent().GetParent().GetParent()).Material;
		shader.SetShaderParameter("camOffset", subpixelCamOffset);
		
		GlobalPosition = realCamPosition;

	}
}
