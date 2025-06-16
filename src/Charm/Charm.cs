using System;
using Godot;

namespace Metroidarium;

public class Charm
{
    protected SnakeHead player;
    public virtual void Init() { }
    
    public virtual void Update() { }
    
    public virtual void Destroy() { }
}

public class DashCharm : Charm
{
    private const float dashSpeedIncrease = 100f;
    private float oldSpeed = 0f;
    
    private bool dashing = false;
    private int dashTime = 20;
    private int dashCounter = 0;
    
    public DashCharm(SnakeHead player, float speed)
    {
        this.player = player;
        oldSpeed = speed;
        player.OnDashedEvent += activateDash;
    }

    private void activateDash()
    {
        dashing = true;
        dashCounter = dashTime;
        player.changeSnakeSpeed(oldSpeed + dashSpeedIncrease);
    }
    
    public override void Update()
    {
        if (dashing)
        {
            dashCounter--;
            if (dashCounter <= 0)
            {
                dashing = false;
                player.changeSnakeSpeed(oldSpeed);
            }
        }
    }

    public override void Destroy()
    {
        player.OnDashedEvent -= activateDash;
    }
    
}

// TODO make it so that different gun charms shoot from different sides
public class GunCharm : Charm
{
    private SnakeTail slot;
    private ShootComponent shooter;

    private Node2D left;
    private Node2D right;
    private Node2D up;
    private Node2D down;
    
    public GunCharm(SnakeHead player, SnakeTail slot)
    {
        this.player = player;
        this.slot = slot;
        left = (Node2D)slot.GetNode("Left");
        right = (Node2D)slot.GetNode("Right");
        up = (Node2D)slot.GetNode("Up");
        down = (Node2D)slot.GetNode("Down");
        
        shooter = new ShootComponent(player.GetParent(), left, "Team Player");
        
        player.OnShotEvent += activateShoot;
    }

    private void activateShoot()
    {
        shooter.Shoot(ToPointMoveComponent.calculateDirection(slot.GlobalPosition, left.GlobalPosition), Bullet.EDataType.Direction);
    }
    
    public override void Destroy()
    {
        player.OnShotEvent -= activateShoot;
    }
    
}