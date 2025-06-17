namespace Metroidarium;


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