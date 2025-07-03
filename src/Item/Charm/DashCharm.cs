using System;
using Godot;

namespace Metroidarium;


public class DashCharm(SnakeHead player, SnakeTail slot) : Charm(player, slot)
{
    private const float DashSpeedIncrease = 100f;
    protected float OldSpeed = 0f;
    
    public bool Dashing = false;
    protected const int DashTime = 20;
    protected int DashCounter = 0;
    
    protected override void activate()
    {
        Player.InAir = true;
        Dashing = true;
        DashCounter = DashTime;
        
        Player.callMethodOnSnake(body => body?.setSpeed(OldSpeed + DashSpeedIncrease));
    }

    protected override void deactivate()
    {
        Player.InAir = false;
        Dashing = false;
        Player.callMethodOnSnake(body => body?.setSpeed(OldSpeed));
    }
    
    public override void Update(float dt)
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
    
    public override void EquipCharm(params Object[] parameters)
    {
        OldSpeed = Player.Speed;
        Player.OnDashedEvent += activate;
    }

    public override void Unequip()
    {
        Player.OnDashedEvent -= activate;
    }
    
}