using System;
using System.Collections.Generic;
using Godot;

namespace Metroidarium;

public class GunCharm(SnakeHead player, SnakeTail slot) : Charm(player, slot)
{
    private Dictionary<Directions, ShootComponent> shooters = new Dictionary<Directions, ShootComponent>();
    private Dictionary<Directions, Node2D> directionPositions;

    public float MaxDelay = 1f;
    private float currentDelay = 0f;
    
    public override void Equip(params Object[] parameters)
    {
        directionPositions = (Dictionary<Directions, Node2D>)parameters[0];
        
        foreach (var direction in directionPositions)
        {
            shooters.Add(direction.Key, createShooter(direction.Value));    
        }
        
        Player.OnShotEvent += activateShoot;
    }

    public override void Update(float dt)
    {
        currentDelay -= dt;
    }
    
    private void activateShoot(Directions direction)
    {
        if(currentDelay > 0) return;
        currentDelay = MaxDelay;
        
        if (direction == Directions.All)
        {
            foreach (var shooter in shooters)
            {
                shoot(shooter.Value, directionPositions[shooter.Key].GlobalPosition);
            }    
        }
        else
        {
            if (directionPositions[direction] != null)
            {
                shoot(shooters[direction],directionPositions[direction].GlobalPosition);    
            }
        }
    }
    
    public override void Unequip()
    {
        Player.OnShotEvent -= activateShoot;
    }
    
    private ShootComponent createShooter(Node2D direction) =>
        new ShootComponent(Player.GetParent(), direction, "Team Player");
    
    private void shoot(ShootComponent shooter, Vector2 targetPos) =>
        shooter.Shoot(ToPointMoveComponent.calculateDirection(slot.GlobalPosition, targetPos), Bullet.EDataType.Direction);

    
}