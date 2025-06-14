using System;
using Godot;

namespace Metroidarium;

public class ShootComponent : Component
{
    private readonly PackedScene bullet = GD.Load<PackedScene>("res://assets/scenes/Bullet.tscn");
    private String teamName;
    private Node game;
    private Node2D shootPoint;
    
    public ShootComponent(Node game, Node2D shootPoint, String teamName)
    {
        this.teamName = teamName;
        this.shootPoint = shootPoint;
        this.game = game;
    }

    public void Shoot(Vector2 vector, Bullet.EDataType vectorType, float speed=50f)
    {
        Bullet newBullet = (Bullet)bullet.Instantiate();
        newBullet.Init(shootPoint.Position, vector, vectorType, teamName);
        game.CallDeferred("add_child", newBullet);
    }
}