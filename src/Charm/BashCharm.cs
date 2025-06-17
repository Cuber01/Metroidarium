namespace Metroidarium;

public class BashCharm : DashCharm
{
    private new const float DashSpeedIncrease = 350f;
    private new const int DashTime = 200;
    public BashCharm(SnakeHead player, float speed) : base(player, speed)
    {
        this.Player = player;
        OldSpeed = speed;
        player.OnDashedEvent += activate;
    }

    protected override void activate()
    {
        base.activate();
        Player.callMethodOnSnake(body => body.SetCollisionLayerValue(2, false));
        Player.callMethodOnSnake(body => body.SetCollisionLayerValue(5, true));
    }
    
    protected override void deactivate()
    {
        base.deactivate();
        Player.callMethodOnSnake(body => body.SetCollisionLayerValue(2, true));
        Player.callMethodOnSnake(body => body.SetCollisionLayerValue(5, false));
    }
}
