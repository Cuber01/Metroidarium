using Godot;
using System;
using Metroidarium;

public partial class VelocityDebugLine : Line2D
{
	private Mob parent;
	public override void _Ready()
	{
		parent = (Mob)GetParent();
	}

	public override void _Process(double delta)
	{
		ClearPoints();
		AddPoint(Vector2.Zero,0);
		AddPoint(parent.Velocity,1);
		GlobalRotation = 0;
	}
}
