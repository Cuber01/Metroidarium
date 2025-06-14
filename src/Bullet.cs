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

	public void Init(Vector2 position, Vector2 vector, EDataType vectorType, String teamName, float speed=50f)
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
		
	}

	private void checkCollision()
	{
		for(int i = GetSlideCollisionCount(); i >= 0; i--)
		{
			Node2D collidingBody = (Node2D)GetSlideCollision(i).GetCollider();
			if (collidingBody is Entity && !collidingBody.IsInGroup(teamName))
			{
				Entity enemy = (Entity)collidingBody;
				enemy.getHurt();
			}
		}
	}
	
}