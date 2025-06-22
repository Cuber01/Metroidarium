using System;
using Godot;

namespace Metroidarium;

public class ShootComponent : Component
{
    private readonly PackedScene bullet = GD.Load<PackedScene>("res://assets/scenes/entities/Bullet.tscn");
    private String teamName;
    private Node game;
    private Node2D shootPoint;
    
    public ShootComponent(Node game, Node2D shootPoint, String teamName)
    {
        this.teamName = teamName;
        this.shootPoint = shootPoint;
        this.game = game;
    }

    public void Shoot(Vector2 vector, Bullet.EDataType vectorType, int damage=1, float speed=50f)
    {
        Bullet newBullet = (Bullet)bullet.Instantiate();
        
        newBullet.Init(shootPoint.GlobalPosition, vector, vectorType, teamName, damage);
        game.CallDeferred("add_child", newBullet);
    }
}