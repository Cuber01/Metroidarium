using System;
using Godot;
using static Godot.Mathf;

namespace Metroidarium;

public partial class Player : Entity
{
	private RayCast2D rayCast;
	private Line2D legs;
	private readonly MoveCommand moveCommand = new MoveCommand();
	private readonly JumpCommand jumpCommand = new JumpCommand();
	
	private bool legsMode;
	private int legsRadius = 200;
	private float legsAngle = 0;
	private float legsAngleSpeed = 0.1f;
	private Vector2 legsPoint = Vector2.Zero;
	
	public override void _Ready()
	{
		rayCast = GetNode<RayCast2D>("RayCast2D");
		legs = GetNode<Line2D>("Legs");
	}
	  
	public override void _PhysicsProcess(double delta)
	{ 
		if (!legsMode)
		{
			normalUpdate(delta);
		}
		else
		{
			legsUpdate();
		}
	}

	private void normalUpdate(double delta)
	{
		velocity = Velocity;
		
		//HandleLegsNormal();
		
		if (!IsOnFloor())
		{
			velocity += GetGravity() * (float)delta;
		}

		if (Input.IsActionJustPressed("ui_up") && IsOnFloor())
		{
			jumpCommand.Execute(this);
		}

		float moveDirection = Input.GetAxis("ui_left", "ui_right");
		if (moveDirection != 0)
		{
			moveCommand.Execute(this, moveDirection);
		}
		else
		{
			velocity.X = MoveToward(Velocity.X, 0, Speed);
		}
			
		Velocity = velocity;
		MoveAndSlide();
	}

	private void legsUpdate()
	{
		Vector2 velocity = Vector2.Zero;
		
		if (Input.IsActionJustPressed("ui_accept"))
		{
			legsMode = false;
			legs.ClearPoints();
			velocity.Y = JumpVelocity;
			Velocity = velocity;
			MoveAndSlide();
			return;
		}
		
		
		Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");
		if(direction != Vector2.Zero)
		{

			// velocity.X += direction.X * Speed/2;
			// velocity.Y += direction.X * Sin((Position.X + velocity.X)/8 ) * 500;
			// GD.Print(Sin(Position.X + velocity.X));
			
			if ( (legsAngle is > -1 and < 1) || (legsAngle >= 1 && direction.X < 0) || (legsAngle <= -1 && direction.X > 0))
			{
				legsAngle += direction.X * legsAngleSpeed;
				velocity.X = direction.X * Cos(legsAngle) * legsRadius;
				velocity.Y = direction.X * Sin(legsAngle) * legsRadius;
				GD.Print(velocity.Y);
			}


		}
		
		Velocity = velocity;
		legs.RemovePoint(1);
		legs.AddPoint(legsPoint);
		MoveAndSlide();
	}
	
	private void HandleLegsNormal()
	{
		if (!legsMode && Input.IsActionJustPressed("ui_accept") && !IsOnFloor() && rayCast.IsColliding())
		{
			legsPoint = ToLocal(rayCast.GetCollisionPoint());
			legs.AddPoint(rayCast.Position);
			legs.AddPoint(legsPoint);
			GD.Print(legsPoint);
			legsMode = true;
			legsAngle = 0;
		}
	}
	
}