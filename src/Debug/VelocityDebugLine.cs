using Godot;
using System;
using Metroidarium;

public partial class VelocityDebugLine : Line2D
{
	[Export] bool on = false;
	
	private Entity parent;
	public override void _Ready()
	{
		if (!on) return;
		parent = (Entity)GetParent();
	}

	public override void _Process(double delta)
	{
		if (!on) return;
		ClearPoints();
		AddPoint(Vector2.Zero,0);
		AddPoint(parent.Velocity,1);
		GlobalRotation = 0;
	}
}
