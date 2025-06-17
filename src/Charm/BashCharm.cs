namespace Metroidarium;

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
