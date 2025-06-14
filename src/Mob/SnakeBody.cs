namespace Metroidarium;

public partial class SnakeBody : Mob
{
    public delegate void GotHitHandler();
    public event GotHitHandler OnGotHitEvent;
    
    protected SnakeBody aheadMe = null;
    public SnakeTail behindMe = null;
    
    public void setSpeed(float speed)
    {
        this.Speed = speed;
    }

    public void makeInvincible()
    {
        healthComponent.MakeInvincible();
    }

    public override void getHurt(int damage)
    {
        OnGotHitEvent!();
        base.getHurt(damage);
    }
    
    protected float Speed;
}