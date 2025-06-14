using Godot;
using System;

public partial class Obstacle : StaticBody2D
{
	// Called when the node enters the scene tree for the first time.


	public override void _Ready()
	{

	}

	private Vector2 motion = new Vector2(0,1);
	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	private int counter = 0;
	public override void _PhysicsProcess(double delta)
	{
		counter += 1;
		if (counter % 100 == 0)
		{
			if (motion == new Vector2(0, 1))
			{
				motion = new Vector2(0, -1);
			}
			else
			{
				motion = new Vector2(0, 1);
			}
		}
		
		MoveAndCollide(motion);
	}
}
