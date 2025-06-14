
using System.Collections.Generic;
using System.Linq;
using Godot;
using static System.Single;
// ReSharper disable InconsistentNaming

namespace Metroidarium;

public partial class SnakeHead : SnakeBody
{
	private readonly PackedScene tailPart = GD.Load<PackedScene>("res://assets/scenes/SnakeTail.tscn");
	
	public delegate void DashedEventHandler();
	public event DashedEventHandler OnDashedEvent;
	
	public delegate void ShotEventHandler();
	public event DashedEventHandler OnShotEvent;
	
	int amountOfTail = 5;
	float rotateSpeed = 0.1f;
	private float deaccelerationSpeed = 1f;
	private SnakeTail behindMe = null;
	
	private List<Charm> charms = new List<Charm>();
	private List<SnakeBody> snake = new List<SnakeBody>();
	
	public override void _Ready()
	{
		healthComponent = new HealthComponent(this,100);
		Speed = 150f;
		OnGotHitEvent += makeSnakeInvincible;
		snake.Add(this);
		
		SnakeBody lastInstance = this;
		for (int i = amountOfTail; i > 0; i--)
		{
			SnakeTail newInstance = (SnakeTail)tailPart.Instantiate();
			GetParent().CallDeferred("add_child", newInstance);
			newInstance.Init(lastInstance);
			lastInstance.behindMe = newInstance;

			newInstance.OnGotHitEvent += makeSnakeInvincible;
			newInstance.OnDeathEvent += removeSnakePart;
			
			snake.Add(newInstance);
			lastInstance = newInstance;
		}


		charms.Add(new DashCharm(this, Speed));
		charms.Add(new GunCharm(this, (SnakeTail)snake[1]));
	}
	
	public override void _PhysicsProcess(double delta)
	{
		
		if (Input.IsActionJustPressed("dash"))
		{
			OnDashedEvent!();
		}
		
		if (Input.IsActionJustPressed("shoot_left"))
		{
			OnShotEvent!();
		}

		foreach (Charm charm in charms)
		{
			charm.Update();
		}
		
		// 8-Pad movement
		Vector2 input = Input.GetVector("left", "right", "up", "down");
		Vector2 newVelocity = new Vector2(input.X * Speed,  input.Y * Speed);
		
		Velocity = newVelocity;
		MoveAndSlide();
	}

	private void makeSnakeInvincible()
	{
		foreach (SnakeBody part in snake)
		{
			part.makeInvincible();
		}
	}
	
	public void changeSnakeSpeed(float speed)
	{
		foreach (SnakeBody part in snake)
		{
			part.setSpeed(speed);
		}
	}

	private void removeSnakePart(SnakeBody part)
	{
		snake.Remove(part);
	}
	
	public override void die()
	{
		if (snake.Count > 1)
		{
			snake[^1].die();
		}
		else
		{
			base.die();	
		}
	}

	private void _OnHurtboxBodyEntered(Node2D body)
	{
		if (body is Enemy)
		{
			GD.Print("OUCH OUCH STOP STOP AAAAAAAA");
		}
	}
}

// Tank Movement
// float rotateDirection = Input.GetAxis("ui_left", "ui_right");
//
// SetRotation(Rotation + rotateSpeed*rotateDirection);
//
// if (Input.IsActionPressed("ui_up"))
// {
// 	float x, y;
// 	x = speed*Cos(Rotation);
// 	y = speed*Sin(Rotation);
// 	velocity = new Vector2(x, y);
// }
// else if (Input.IsActionPressed("ui_down"))
// {
// 	velocity = velocity.MoveToward(Vector2.Zero, deaccelerationSpeed*3);
// }
// else if (velocity != Vector2.Zero)
// {
// 	velocity = velocity.MoveToward(Vector2.Zero, deaccelerationSpeed);
// }