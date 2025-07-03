using System;
using System.Collections.Generic;
using System.Linq;
using Godot;

// ReSharper disable InconsistentNaming

namespace Metroidarium;

public partial class SnakeHead : SnakeBody
{
	#region Constructors and properties
	
	private readonly PackedScene tailPart = GD.Load<PackedScene>("res://assets/scenes/entities/SnakeTail.tscn");

	public delegate void InteractedEventHandler(SnakeHead player);
	public event InteractedEventHandler OnInteractedEvent;
	
	public delegate void DashedEventHandler(); 
	public event DashedEventHandler OnDashedEvent;
	
	public delegate void ShotEventHandler(Directions direction);
	public event ShotEventHandler OnShotEvent;

	private Vector2 lastChangedVelocity = new Vector2(0,1);
	private Vector2 lastPositionOnGround;

	// TODO add real state - this is set by dash charm
	public bool InAir = false;
	
	private int currentAmountOfTail = 0;

	public int CurrentAmountOfTail
	{
		get
		{
			return SnakeParts.Count(part => part != null) - 1;
		}
	}	
	
	private int amountOfTail;
	public int AmountOfTail {
		get => amountOfTail;
		set
		{
			#if DEBUG_TAIL
				debugPrint($"Changed amountOfTail from {amountOfTail} to {value}");
			#endif
			
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
	
	public readonly List<Charm> charms = new List<Charm>(10);
	public readonly List<Charm> permanentCharms = new List<Charm>(10);

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
		AddComponent(new TilemapToolsComponent());
		AddComponent(new TweenComponent(this));
		charms.Add(null);
		
		Speed = 150f;
		OnGotHitEvent += () => callMethodOnSnake(body => body?.makeInvincible());
		SnakeParts.Add(this);
		PartId = 0;
		AmountOfTail = 3;
		growTail(amountOfTail);

		InventoryItem gunCharm = ResourceLoader.Load<InventoryItem>("res://assets/item_data/charms/double_cannon.tres");
		InventoryItem assault = ResourceLoader.Load<InventoryItem>("res://assets/item_data/charms/assault_rightle.tres");
		InventoryItem dash = ResourceLoader.Load<InventoryItem>("res://assets/item_data/charms/dash_charm.tres");
		
		GetComponent<InventoryComponent>().AddItem(gunCharm);
		GetComponent<InventoryComponent>().AddItem(assault);
		GetComponent<InventoryComponent>().AddItem(dash);
	}
	
	#endregion

	#region Update loop
	
	public override void _PhysicsProcess(double delta)
	{
		GetComponent<HealthComponent>().updateInvincibility((float)delta);
		
		handleFalling();
		handleShooting();
		updateCharms((float)delta);
		handleMoving();
		
		handleEvents();
		MoveAndSlide();
	}

	private void handleShooting()
	{
		foreach (var entry in actionToDirection)
		{
			if (Input.IsActionPressed(entry.Key))
			{
				OnShotEvent?.Invoke(entry.Value);
				break;
			}
		}
	}

	private void updateCharms(float delta)
	{
		foreach (Charm charm in charms)
		{
			charm?.Update(delta);
		}

		foreach (Charm charm in permanentCharms)
		{
			charm.Update(delta);
		}
	}

	private void handleMoving()
	{
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
	}

	private void handleEvents()
	{
		if (Input.IsActionJustPressed("dash"))
		{
			OnDashedEvent?.Invoke();
		}
		
		if (Input.IsActionJustPressed("interact"))
		{
			if (OnInteractedEvent != null)
			{
				InAir = false;
				OnInteractedEvent?.Invoke(this);
				GetComponent<InventoryComponent>().OpenMenu();	
			}
		}
	}
	
	private void handleFalling()
	{
		if (!InAir)
		{
			// Standing on hole
			if (GetComponent<TilemapToolsComponent>().IsHole(this, Position))
			{
				Fall();
			}
			// Standing on ground
			else
			{
				lastPositionOnGround = Position;	
			}
		}
	}

	protected override void Fall()
	{
		base.Fall();
		SetProcessInput(false);
	}

	protected override void EndFalling()
	{
		getHurt(1);
		SetScale(Vector2.One);
		Position = lastPositionOnGround;
		GD.Print(lastPositionOnGround);
		SetProcessInput(true);
	}
	
	#endregion
	
	#region Handling tail
	
	public void callMethodOnTail(Action<SnakeTail> method)
	{
		foreach (SnakeBody part in SnakeParts)
		{
			if (part is SnakeTail tail)
			{
				method(tail);	
			}
		}
	}

	public void callMethodOnSnake(Action<SnakeBody> method)
	{
		foreach (SnakeBody part in SnakeParts)
		{
			method(part);
		}
	}

	private void removeSnakePart(int partId)
	{
		SnakeParts[partId] = null;
		
		if (charms[partId] != null)
		{
			charms[partId].Unequip();
			charms[partId] = null;	
			
			#if DEBUG_TAIL
			GD.Print($"Unequipped charm at {partId} and replaced it with null.");
			#endif
		}
		
		#if DEBUG_TAIL
		debugPrint($"Removed snake part at {partId} and replaced it with null.");
		#endif
	}

	public void growTail(int amount)
	{
		#if DEBUG_TAIL
		GD.Print($"Growing tail by {amount}.");
		#endif

		for (int j = 0; j < amount; j++)
		{
			// Calculate new index
			bool replacing = false;
			int newIndex = -1;
			for (int i = 0; i < SnakeParts.Count; i++)
			{
				if (SnakeParts[i] == null)
				{
					newIndex = i;
					replacing = true;
					break;
				}
			}
			if (newIndex == -1)
			{
				newIndex = SnakeParts.Count;
				replacing = false;
			}
		
			#if DEBUG_TAIL
			GD.Print($"New tail's index is {newIndex}.");
			#endif

			// Instantiate new tail
			SnakeTail newInstance = (SnakeTail)tailPart.Instantiate();

			// Add tail to snakeParts
			if (replacing)
			{
				SnakeParts[newIndex] = newInstance;
				
				#if DEBUG_TAIL
				debugPrint($"Replaced null by new tail at {newIndex}.");
				#endif
			}
			else
			{
				SnakeParts.Add(newInstance);
				#if DEBUG_TAIL
				debugPrint($"Grown the tail to include {newIndex} id.");
				#endif
			}
			
			//Init
			newInstance.Init(ref SnakeParts, newIndex);
			GetParent().CallDeferred("add_child", newInstance);
			
			newInstance.OnGotHitEvent += () => callMethodOnSnake(body => body?.makeInvincible());
			newInstance.OnDeathEvent += removeSnakePart;
		}
		
	}
	
	#endregion
	
	#region Callbacks
	
	// Called by Pickupable Item
	public void pickup(InventoryItem item)
	{
		GetComponent<InventoryComponent>().AddItem(item);
	}

	protected override void _onHurtboxBodyEntered(Node2D body)
	{
		if (body is TileMapLayer layer && layer.Name == "Hole")
		{
			Fall();	
		}
		else
		{
			base._onHurtboxBodyEntered(body);	
		}
	}

	public override void die()
	{
		for (int j = SnakeParts.Count - 1; j >= 0; j--)
		{
			// All other parts are already dead
			if (SnakeParts[j] is SnakeHead)
			{
				base.die();
			// Else kill the last part	
			} else if (SnakeParts[j] != null)
			{
				SnakeParts[j].die();
				removeSnakePart(j);
				break;
			}
		}
	}
	
	#endregion
	
	#if DEBUG_TAIL
	private void debugPrint(string msg)
	{
		GD.Print(msg);
		GD.PrintRich(this);
	}
	
	public override string ToString()
	{
		string rv = "";
		foreach (SnakeBody part in snakeParts)
		{
			if (part == null)
			{
				rv += "[color=purple]Null[/color] ";
			} else if (part is SnakeTail)
			{
				rv += "[color=green]Tail[/color] ";
			} else if (part is SnakeHead)
			{
				rv += "[color=blue]Head[/color] ";
			}
		}
		return rv;
	}
	#endif
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