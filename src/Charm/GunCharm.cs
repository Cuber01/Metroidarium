using System.Collections.Generic;
using Godot;

namespace Metroidarium;

public class GunCharm : Charm
{
    private SnakeTail slot;
    private Dictionary<Directions, ShootComponent> shooters = new Dictionary<Directions, ShootComponent>();
    private DirectionPositions directions;

    private enum Directions
    {
        Left,
        Right,
        Up,
        Down
    }
    
    public struct DirectionPositions(Node2D left = null, Node2D right = null, Node2D up = null, Node2D down = null)
    {
        public readonly Node2D Left = left;
        public readonly Node2D Right = right;
        public readonly Node2D Up = up;
        public readonly Node2D Down = down;
    }
    
    public GunCharm(SnakeHead player, SnakeTail slot, DirectionPositions directions)
    {
        this.Player = player;
        this.slot = slot;
        this.directions = directions;

        if (directions.Left != null)
            shooters.Add(Directions.Left, createShooter(directions.Left));
        if (directions.Right != null)
            shooters.Add(Directions.Right, createShooter(directions.Right));
        if (directions.Up != null)
            shooters.Add(Directions.Up, createShooter(directions.Up));
        if (directions.Down != null)
            shooters.Add(Directions.Down, createShooter(directions.Down));
        
        player.OnShotEvent += activateShoot;
    }

    private ShootComponent createShooter(Node2D direction) =>
        new ShootComponent(Player.GetParent(), direction, "Team Player");
    
    private void shoot(ShootComponent shooter, Vector2 targetPos) =>
        shooter.Shoot(ToPointMoveComponent.calculateDirection(slot.GlobalPosition, targetPos), Bullet.EDataType.Direction);

    private void activateShoot()
    {
        foreach (var shooter in shooters)
        {
            var shootComponent = shooter.Value;
            switch (shooter.Key)
            {
                case Directions.Left:
                    shoot(shootComponent, directions.Left.GlobalPosition);
                    break;
                case Directions.Right:
                    shoot(shootComponent, directions.Right.GlobalPosition);
                    break;
                case Directions.Up:
                    shoot(shootComponent, directions.Up.GlobalPosition);
                    break;
                case Directions.Down:
                    shoot(shootComponent, directions.Down.GlobalPosition);
                    break;
            }
        }
    }
    
    public override void Destroy()
    {
        Player.OnShotEvent -= activateShoot;
    }
    
}