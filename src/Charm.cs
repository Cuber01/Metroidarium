using System;
using Godot;

namespace Metroidarium;

public class Charm
{
    public virtual void Init() { }
    
    public virtual void Update() { }
}

public class DashCharm : Charm
{
    private SnakeHead player;
    
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
        player.changeSpeed(oldSpeed + dashSpeedIncrease);
    }
    
    public override void Update()
    {
        if (dashing)
        {
            dashCounter--;
            if (dashCounter <= 0)
            {
                dashing = false;
                player.changeSpeed(oldSpeed);
            }
        }
    }
    
}