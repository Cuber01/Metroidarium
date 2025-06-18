using System;
using Godot;

namespace Metroidarium;

public partial class Bullet : Entity
{
	public enum EDataType
	{
		TargetPosition,
		Direction
	}

	private float speed = 50f;
	public String TeamName;

	public void Init(Vector2 position, Vector2 vector, EDataType vectorType, String teamName, int damage, float speed=75f)
	{
		AddComponent(new DirectionalMoveComponent(this, 
			vectorType == EDataType.Direction ?
			vector :
			ToPointMoveComponent.calculateDirection(position, vector),
			speed));
		AddComponent(new ContactComponent(damage));
		this.Position = position;
		this.TeamName = teamName;
		this.speed = speed;
		AddToGroup(teamName);

		if (teamName == "Team Player")
		{
			SetCollisionLayerValue(5, true);
		}
		else if (teamName == "Team Baddies")
		{
			SetCollisionLayerValue(4, true);
		}
		else throw new ArgumentException();
	}

	public override void _PhysicsProcess(double delta)
	{
		GetComponent<DirectionalMoveComponent>().update();
		MoveAndSlide();
		checkCollision();
	}

	private void checkCollision()
	{
		int colCount = GetSlideCollisionCount();

		if (colCount > 0)
		{
			die();
		}
	}

	public void die()
	{
		CallDeferred("queue_free");
	}
	
}