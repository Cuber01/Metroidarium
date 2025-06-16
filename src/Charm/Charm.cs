using System;
using System.Collections.Generic;
using Godot;

namespace Metroidarium;

public class Charm
{
    protected SnakeHead player;
    
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
        this.player = player;
        OldSpeed = speed;
        player.OnDashedEvent += activate;
    }

    protected virtual void activate()
    {
        Dashing = true;
        DashCounter = DashTime;
        player.callMethodOnSnake(body => body.setSpeed(OldSpeed + DashSpeedIncrease));
    }

    protected virtual void deactivate()
    {
        Dashing = false;
        player.callMethodOnSnake(body => body.setSpeed(OldSpeed));
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
        player.OnDashedEvent -= activate;
    }
    
}

// TODO finish this when you have a proper state system in place
public class BashCharm : DashCharm
{
    private new const float DashSpeedIncrease = 200f;
    private new const int DashTime = 50;
    public BashCharm(SnakeHead player, float speed) : base(player, speed)
    {
        this.player = player;
        OldSpeed = speed;
        player.OnDashedEvent += activate;
    }

    protected override void activate()
    {
        base.activate();
        player.callMethodOnSnake(body => body.makeInvincible());
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