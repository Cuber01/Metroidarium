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

	public void Init(Vector2 position, Vector2 vector, EDataType vectorType, String teamName, int damage, float speed = 75f)
	{
		AddComponent(new DirectionalMoveComponent(this, 
			vectorType == EDataType.Direction ?
			vector :
			ToPointMoveComponent.calculateDirection(position, vector),
			speed));
		AddComponent(new ContactComponent(damage));
		AddComponent(new ShaderComponent(this));
		this.Position = position;
		this.TeamName = teamName;
		this.speed = speed;
		AddToGroup(teamName);

		if (teamName == "Team Player")
		{
			SetCollisionLayerValue(5, true);
			GetComponent<ShaderComponent>().Shader.SetShaderParameter("green_tint", 0.5f);
		}
		else if (teamName == "Team Baddies")
		{
			SetCollisionLayerValue(4, true);
			GetComponent<ShaderComponent>().Shader.SetShaderParameter("red_tint", 1.0);
			GetComponent<ShaderComponent>().Shader.SetShaderParameter("blue_tint", 0.0);
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