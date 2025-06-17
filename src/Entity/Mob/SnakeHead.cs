
using System;
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
	
	public Vector2 lastChangedVelocity = new Vector2(0,1);
	
	int amountOfTail = 5;
	float rotateSpeed = 0.1f;
	private float deaccelerationSpeed = 1f;
	private SnakeTail behindMe = null;
	
	private List<Charm> charms = new List<Charm>();
	private List<SnakeBody> snake = new List<SnakeBody>();
	
	public override void _Ready()
	{
		AddComponent(new HealthComponent(this,1));
		charms = Enumerable.Repeat<Charm>(null, amountOfTail+1).ToList();

		
		Speed = 150f;
		OnGotHitEvent += () => callMethodOnSnake(body => body?.makeInvincible());
		snake.Add(this);
		PartId = 0;
		
		SnakeBody lastInstance = this;
		for (int i = amountOfTail; i > 0; i--)
		{
			SnakeTail newInstance = (SnakeTail)tailPart.Instantiate();
			GetParent().CallDeferred("add_child", newInstance);
			newInstance.Init(lastInstance, amountOfTail-i+1);
			lastInstance.BehindMe = newInstance;

			newInstance.OnGotHitEvent += () => callMethodOnSnake(body => body?.makeInvincible());
			newInstance.OnDeathEvent += removeSnakePart;
			
			snake.Add(newInstance);
			lastInstance = newInstance;
		}


		charms[0] = new BashCharm(this, Speed);
		SnakeTail slot = (SnakeTail)snake[5];
		charms[1] = new GunCharm(this, slot, new GunCharm.DirectionPositions((Node2D)slot.GetNode("Left"),
			(Node2D)slot.GetNode("Right"),null,(Node2D)slot.GetNode("Down")));
	}
	
	public override void _PhysicsProcess(double delta)
	{
		if (Input.IsActionJustPressed("shoot_left"))
		{
			OnShotEvent?.Invoke();
		}

		foreach (Charm charm in charms)
		{
			charm?.Update();
		}
		
		// 8-Pad movement
		Vector2 input = Input.GetVector("left", "right", "up", "down");

		if (input.X != 0 || input.Y != 0)
		{
			lastChangedVelocity = new Vector2(input.X * Speed,  input.Y * Speed);
			SetRotation(lastChangedVelocity.Angle());
			Velocity = lastChangedVelocity;
		}
		else
		{
			Velocity = Vector2.Zero;
		}
		
		// Dashing
		if (Input.IsActionJustPressed("dash"))
		{
			OnDashedEvent?.Invoke();
		}
		
		MoveAndSlide();
	}

	public void callMethodOnSnake(Action<SnakeBody> method)
	{
		foreach (SnakeBody part in snake)
		{
			method(part);
		}
	}

	private void removeSnakePart(int partId)
	{
		snake[partId] = null;

		if (charms[partId] != null)
		{
			charms[partId].Destroy();
			charms[partId] = null;	
		}
	}
	
	public override void die()
	{

		for (int j = snake.Count - 1; j >= 0; j--)
		{
			
			// All other parts are already dead
			if (snake[j] is SnakeHead)
			{
				base.die();
				
			// Else kill the last part	
			} else if (snake[j] != null)
			{
				snake[j].die();
				break;
			}
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