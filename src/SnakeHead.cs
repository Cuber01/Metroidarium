using Godot;
using static System.Single;
// ReSharper disable InconsistentNaming

namespace Metroidarium;

public partial class SnakeHead : Entity
{
	PackedScene tailPart = GD.Load<PackedScene>("res://SnakeBody.tscn");
	
	int partID = 0;
	int amountOfTail = 5;
	float rotateSpeed = 0.1f;
	float speed = 150f;
	private float deaccelerationSpeed = 1f;

	public override void _Ready()
	{
		Entity toFollow = this;
		for (int i = 1; i <= amountOfTail; i++)
		{
			SnakeBody instance = (SnakeBody)tailPart.Instantiate();
			instance.Init(toFollow, i);
			GetParent().CallDeferred("add_child", instance);
			toFollow = instance;
		}
	}

	
	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;
		float rotateDirection = Input.GetAxis("ui_left", "ui_right");

		SetRotation(Rotation + rotateSpeed*rotateDirection);

		if (Input.IsActionPressed("ui_up"))
		{
			float x, y;
			x = speed*Cos(Rotation);
			y = speed*Sin(Rotation);
			velocity = new Vector2(x, y);
		}
		else if (Input.IsActionPressed("ui_down"))
		{
			velocity = velocity.MoveToward(Vector2.Zero, deaccelerationSpeed*3);
		}
		else if (velocity != Vector2.Zero)
		{
			velocity = velocity.MoveToward(Vector2.Zero, deaccelerationSpeed);
		}
		
		
		Velocity = velocity;
		MoveAndSlide();
	}
}