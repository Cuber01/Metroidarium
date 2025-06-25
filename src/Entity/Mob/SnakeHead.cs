
using System;
using System.Collections.Generic;
using Godot;

// ReSharper disable InconsistentNaming

namespace Metroidarium;

public partial class SnakeHead : SnakeBody
{
	private readonly Godot.PackedScene tailPart = GD.Load<Godot.PackedScene>("res://assets/scenes/entities/SnakeTail.tscn");

	public delegate void InteractedEventHandler(SnakeHead player);
	public event InteractedEventHandler OnInteractedEvent;
	
	public delegate void DashedEventHandler(); 
	public event DashedEventHandler OnDashedEvent;
	
	public delegate void ShotEventHandler(Directions direction);
	public event ShotEventHandler OnShotEvent;
	
	public Vector2 lastChangedVelocity = new Vector2(0,1);

	// TODO will need to implement a proper state machine if needed
	public bool restState = false;
	
	public int CurrentAmountOfTail = 0;
	private int amountOfTail;
	public int AmountOfTail {
		get => amountOfTail;
		set
		{
			int change = Math.Abs(amountOfTail-value);
			while (change > 0)
			{
				charms.Add(null);	
				change--;
			}
			amountOfTail = value;
		}
	}
	
	private float rotateSpeed = 0.1f;
	private float deaccelerationSpeed = 1f;
	private SnakeTail behindMe = null;
	
	public List<Charm> charms = new List<Charm>();
	public List<Charm> pernamentCharms = new List<Charm>();
	public List<SnakeBody> snakeParts = new List<SnakeBody>();

	private ItemLoader ItemLoader;
	
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
		ItemLoader = new ItemLoader("res://assets/item_data/charms/");
		AddComponent(new ContactComponent(5));
		AddComponent(new InventoryComponent(this, ItemLoader.AllCharms));
		charms.Add(null);
		
		Speed = 150f;
		OnGotHitEvent += () => callMethodOnSnake(body => body?.makeInvincible());
		snakeParts.Add(this);
		PartId = 0;
		AmountOfTail = 3;
		
		growTail(2);

		InventoryItem gunCharm = ResourceLoader.Load<InventoryItem>("res://assets/item_data/charms/double_cannon.tres");
		InventoryItem assault = ResourceLoader.Load<InventoryItem>("res://assets/item_data/charms/assault_rightle.tres");
		
		GetComponent<InventoryComponent>().AddItem(gunCharm);
		GetComponent<InventoryComponent>().AddItem(assault);
	}
	
	public override void _PhysicsProcess(double delta)
	{
		GetComponent<HealthComponent>().updateInvincibility((float)delta);

		foreach (var entry in actionToDirection)
		{
			if (Input.IsActionPressed(entry.Key))
			{
				OnShotEvent?.Invoke(entry.Value);
				break;
			}
		}

		foreach (Charm charm in charms)
		{
			charm?.Update((float)delta);
		}

		foreach (Charm charm in pernamentCharms)
		{
			charm.Update((float)delta);
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
		
		if (Input.IsActionJustPressed("interact"))
		{
			if (OnInteractedEvent != null)
			{ 
				OnInteractedEvent?.Invoke(this);
				GetComponent<InventoryComponent>().OpenMenu();	
			}
		}
	}

	public void callMethodOnTail(Action<SnakeTail> method)
	{
		foreach (SnakeBody part in snakeParts)
		{
			if (part is SnakeTail tail)
			{
				method(tail);	
			}
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
		
		CurrentAmountOfTail--;
	}

	public void growTail(int amount)
	{
		SnakeBody lastInstance = null;
		int lastIndex = -1;
		for (int i = 0; i < snakeParts.Count; i++)
		{
			if (snakeParts[i] != null)
			{
				lastInstance = snakeParts[i];
				lastIndex = i;
			}
		}
		
		for (int i = 0; i < amount; i++)
		{
			SnakeTail newInstance = (SnakeTail)tailPart.Instantiate();
			GetParent().CallDeferred("add_child", newInstance);
			newInstance.Init(lastInstance, lastIndex+i);
			lastInstance!.BehindMe = newInstance;

			newInstance.OnGotHitEvent += () => callMethodOnSnake(body => body?.makeInvincible());
			newInstance.OnDeathEvent += removeSnakePart;
			
			snakeParts.Add(newInstance);
			lastInstance = newInstance;
		}
		
		CurrentAmountOfTail += amount;
	}
	
	// Called by Pickupable Item
	public void pickup(InventoryItem item)
	{
		GetComponent<InventoryComponent>().AddItem(item);
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