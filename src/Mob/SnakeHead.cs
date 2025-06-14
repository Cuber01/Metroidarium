
using System.Collections.Generic;
using System.Linq;
using Godot;
using static System.Single;
// ReSharper disable InconsistentNaming

namespace Metroidarium;

public partial class SnakeHead : Mob
{
	private readonly PackedScene tailPart = GD.Load<PackedScene>("res://assets/scenes/SnakeBody.tscn");
	
	public delegate void DashedEventHandler();
	public event DashedEventHandler OnDashedEvent;
	
	public delegate void ShotEventHandler();
	public event DashedEventHandler OnShotEvent;
	
	int amountOfTail = 5;
	float rotateSpeed = 0.1f;
	private float speed = 150f;
	private float deaccelerationSpeed = 1f;
	private SnakeTail behindMe = null;
	
	private List<Charm> charms = new List<Charm>();
	
	public override void _Ready()
	{
		behindMe = (SnakeTail)tailPart.Instantiate();
		GetParent().CallDeferred("add_child", behindMe);
		behindMe.Init(this, amountOfTail - 1, (Node2D)GetParent());

		charms.Add(new DashCharm(this, speed));
		charms.Add(new GunCharm(this, behindMe));
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
		Vector2 newVelocity = new Vector2(input.X * speed,  input.Y * speed);
		
		Velocity = newVelocity;
		MoveAndSlide();
	}
	
	public void changeSpeed(float speed)
	{
		this.speed = speed;
		if (behindMe != null)
		{
			behindMe.changeSpeed(speed);    
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