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

	DirectionalMoveComponent moveComponent;
	private float speed = 50f;
	public String teamName;

	public void Init(Vector2 position, Vector2 vector, EDataType vectorType, String teamName, int damage, float speed=50f)
	{
		moveComponent = new DirectionalMoveComponent(this, 
			vectorType == EDataType.Direction ?
			vector :
			ToPointMoveComponent.calculateDirection(Position, vector),
			speed);
		this.Position = position;
		this.teamName = teamName;
		this.speed = speed;
		AddToGroup(teamName);
	}

	public override void _PhysicsProcess(double delta)
	{
		moveComponent.update();
		Velocity = velocity;
		MoveAndSlide();
		checkCollision();
	}

	private void checkCollision()
	{
		int colCount = GetSlideCollisionCount();
		for(int i = colCount; i > 0; i--)
		{
			Node2D collidingBody = (Node2D)GetSlideCollision(i-1).GetCollider();
			if (collidingBody is Entity enemy && !enemy.IsInGroup(teamName))
			{
				enemy.getHurt();
			}
		}

		if (colCount > 0)
		{
			die();
		}
	}
	
}