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
	private int damage;
	public String TeamName;

	public void Init(Vector2 position, Vector2 vector, EDataType vectorType, String teamName, int damage, float speed=75f)
	{
		AddComponent(new DirectionalMoveComponent(this, 
			vectorType == EDataType.Direction ?
			vector :
			ToPointMoveComponent.calculateDirection(position, vector),
			speed));
		this.Position = position;
		this.TeamName = teamName;
		this.speed = speed;
		this.damage = damage;
		AddToGroup(teamName);

		if (teamName == "Team Player")
		{
			SetCollisionMaskValue(3, true);
		}
		else if (teamName == "Team Baddies")
		{
			SetCollisionMaskValue(2, true);
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
		for(int i = colCount; i > 0; i--)
		{
			Node2D collidingBody = (Node2D)GetSlideCollision(i-1).GetCollider();
			if (collidingBody is Mob enemy)
			{
				enemy.getHurt(damage);
				break;
			}
		}

		if (colCount > 0)
		{
			die();
		}
	}

	private void die()
	{
		CallDeferred("queue_free");
	}
	
}