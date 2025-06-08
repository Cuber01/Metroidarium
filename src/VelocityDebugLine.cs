using Godot;
using System;
using Metroidarium;

public partial class VelocityDebugLine : Line2D
{
	private Entity parent;
	public override void _Ready()
	{
		parent = (Entity)GetParent();
	}

	public override void _Process(double delta)
	{
		ClearPoints();
		AddPoint(Vector2.Zero,0);
		AddPoint(parent.velocity,1);
		GlobalRotation = 0;
	}
}
