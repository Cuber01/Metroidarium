using Godot;

namespace Metroidarium;

public class BashCharm(SnakeHead player, SnakeTail slot) : DashCharm(player, slot)
{
    private new const float DashSpeedIncrease = 350f;
    private new const int DashTime = 200;

    protected override void activate()
    {
        base.activate();
        Player.callMethodOnSnake(body => body?.SetCollisionLayerValue(2, false));
        Player.callMethodOnSnake(body => body?.SetCollisionLayerValue(5, true));
        Player.callMethodOnSnake(body =>
        {
            Area2D hurtbox = (Area2D)body?.GetNode("Hurtbox");
            hurtbox?.SetCollisionMaskValue(4, false);
        });
    }
    
    protected override void deactivate()
    {
        base.deactivate();
        Player.callMethodOnSnake(body => body?.SetCollisionLayerValue(2, true));
        Player.callMethodOnSnake(body => body?.SetCollisionLayerValue(5, false));
        Player.callMethodOnSnake(body =>
        {
            Area2D hurtbox = (Area2D)body?.GetNode("Hurtbox");
            hurtbox?.SetCollisionMaskValue(4, true);
        });
    }
}
