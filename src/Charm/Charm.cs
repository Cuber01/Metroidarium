using System;
using System.Collections.Generic;
using Godot;

namespace Metroidarium;

public class Charm
{
    protected SnakeHead Player;
    
    public virtual void Init() { }
    
    public virtual void Update() { }
    
    public virtual void Destroy() { }
    
    protected virtual void activate() { }
    
    protected virtual void deactivate() { }
}

public class DashCharm : Charm
{
    protected const float DashSpeedIncrease = 100f;
    protected float OldSpeed = 0f;
    
    protected bool Dashing = false;
    protected const int DashTime = 20;
    protected int DashCounter = 0;
    
    public DashCharm(SnakeHead player, float speed)
    {
        this.Player = player;
        OldSpeed = speed;
        player.OnDashedEvent += activate;
    }

    protected virtual void activate()
    {
        Dashing = true;
        DashCounter = DashTime;
        Player.callMethodOnSnake(body => body.setSpeed(OldSpeed + DashSpeedIncrease));
    }

    protected virtual void deactivate()
    {
        Dashing = false;
        Player.callMethodOnSnake(body => body.setSpeed(OldSpeed));
    }
    
    public override void Update()
    {
        if (Dashing)
        {
            DashCounter--;
            if (DashCounter <= 0)
            {
                deactivate();
            }
        }
    }

    public override void Destroy()
    {
        Player.OnDashedEvent -= activate;
    }
    
}

// TODO finish this when you have a proper state system in place
public class BashCharm : DashCharm
{
    private new const float DashSpeedIncrease = 200f;
    private new const int DashTime = 50;
    public BashCharm(SnakeHead player, float speed) : base(player, speed)
    {
        this.Player = player;
        OldSpeed = speed;
        player.OnDashedEvent += activate;
    }

    protected override void activate()
    {
        base.activate();
        Player.callMethodOnSnake(body => body.makeInvincible());
    }
    
    protected override void deactivate()
    {
        base.deactivate();
    }
}

// TODO make it so that different gun charms shoot from different sides
public class GunCharm : Charm
{
    private SnakeTail slot;
    private Dictionary<Directions, ShootComponent> shooters = new Dictionary<Directions, ShootComponent>();
    private DirectionPositions directions;

    public enum Directions
    {
        Left,
        Right,
        Up,
        Down
    }
    
    public struct DirectionPositions(Node2D left = null, Node2D right = null, Node2D up = null, Node2D down = null)
    {
        public Node2D Left = left;
        public Node2D Right = right;
        public Node2D Up = up;
        public Node2D Down = down;
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