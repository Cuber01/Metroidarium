
using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Metroidarium.Menu;
using static System.Single;
// ReSharper disable InconsistentNaming

namespace Metroidarium;

public partial class SnakeHead : SnakeBody
{
	private readonly PackedScene tailPart = GD.Load<PackedScene>("res://assets/scenes/entities/SnakeTail.tscn");
	
	public delegate void DashedEventHandler();
	public event DashedEventHandler OnDashedEvent;
	
	public delegate void ShotEventHandler(Directions direction);
	public event ShotEventHandler OnShotEvent;
	
	public Vector2 lastChangedVelocity = new Vector2(0,1);

	private static int amountOfTail = 5;
	private float rotateSpeed = 0.1f;
	private float deaccelerationSpeed = 1f;
	private SnakeTail behindMe = null;
	
	public List<Charm> charms = new List<Charm>();
	public List<SnakeBody> snakeParts = new List<SnakeBody>();
	
	private readonly Dictionary<String, Directions> actionToDirection = new()
	{
		{ "shoot_left", Directions.Left },
		{ "shoot_right", Directions.Right },
		{ "shoot_up", Directions.Up },
		{ "shoot_down", Directions.Down }
	};
	
	public override void _Ready()
	{
		base._Ready();
		AddComponent(new ContactComponent(5));
		AddComponent(new InventoryComponent(this));
		charms = Enumerable.Repeat<Charm>(null, amountOfTail+1).ToList();
		
		Speed = 150f;
		OnGotHitEvent += () => callMethodOnSnake(body => body?.makeInvincible());
		snakeParts.Add(this);
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
			
			snakeParts.Add(newInstance);
			lastInstance = newInstance;
		}
		
		
		InventoryItem dashCharm = ResourceLoader.Load<InventoryItem>("res://assets/item_data/dash_charm.tres");
		InventoryItem gunCharm = ResourceLoader.Load<InventoryItem>("res://assets/item_data/double_cannon.tres");
		InventoryItem assault = ResourceLoader.Load<InventoryItem>("res://assets/item_data/assault_rightle.tres");
		
		GetComponent<InventoryComponent>().AddItem(gunCharm);
		GetComponent<InventoryComponent>().AddItem(dashCharm);
		GetComponent<InventoryComponent>().AddItem(assault);

		// charms[0] = new BashCharm(this, Speed);
		// SnakeTail slot = (SnakeTail)snakeParts[5];
		// charms[5] = new GunCharm(this, slot, new Dictionary<Directions, Node2D> {
		// 		{Directions.Left, (Node2D)slot.GetNode("Left")},
		// 		{Directions.Right, (Node2D)slot.GetNode("Right")},
		// 		{Directions.Up, null},
		// 		{Directions.Down, (Node2D)slot.GetNode("Down")}
		// 	});
		// SnakeTail slot2 = (SnakeTail)snakeParts[4];
		// charms[4] = new GunCharm(this, slot2, new Dictionary<Directions, Node2D> {
		// 	{Directions.Left, (Node2D)slot2.GetNode("Left")},
		// 	{Directions.Right, (Node2D)slot2.GetNode("Right")},
		// 	{Directions.Up, null},
		// 	{Directions.Down, (Node2D)slot2.GetNode("Down")}
		// });
		// SnakeTail slot3 = (SnakeTail)snakeParts[3];
		// charms[3] = new GunCharm(this, slot3, new Dictionary<Directions, Node2D> {
		// 	{Directions.Left, (Node2D)slot3.GetNode("Left")},
		// 	{Directions.Right, (Node2D)slot3.GetNode("Right")},
		// 	{Directions.Up, null},
		// 	{Directions.Down, (Node2D)slot3.GetNode("Down")}
		// });
	}
	
	public override void _PhysicsProcess(double delta)
	{
		GetComponent<HealthComponent>().updateInvincibility((float)delta);

		foreach (var entry in actionToDirection)
		{
			if (Input.IsActionJustPressed(entry.Key))
			{
				OnShotEvent?.Invoke(entry.Value);
				break;
			}
		}

		foreach (Charm charm in charms)
		{
			charm?.Update((float)delta);
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
		
		if (Input.IsActionJustPressed("inventory"))
		{
			GetComponent<InventoryComponent>().OpenMenu();
		}
	}

	public void callMethodOnSnake(Action<SnakeBody> method)
	{
		foreach (SnakeBody part in snakeParts)
		{
			method(part);
		}
	}

	private void removeSnakePart(int partId)
	{
		snakeParts[partId] = null;
		
		if (charms[partId] != null)
		{
			charms[partId].Unequip();
			charms[partId] = null;	
		}
	}
	
	public override void die()
	{
		for (int j = snakeParts.Count - 1; j >= 0; j--)
		{
			// All other parts are already dead
			if (snakeParts[j] is SnakeHead)
			{
				base.die();
			// Else kill the last part	
			} else if (snakeParts[j] != null)
			{
				snakeParts[j].die();
				removeSnakePart(j);
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