namespace Metroidarium;

public partial class SnakeBody : Mob
{
    public delegate void GotHitHandler();
    public event GotHitHandler OnGotHitEvent;
    
    public delegate void DiedHandler(SnakeBody part);
    public event DiedHandler OnDeathEvent;
    
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
    
    public override void die()
    {
        if (behindMe != null)
        {
            behindMe.aheadMe = aheadMe;
        }
        aheadMe.behindMe = behindMe;
        OnDeathEvent!(this);
        base.die();
    }

    public override void getHurt(int damage)
    {
        base.getHurt(damage);
        OnGotHitEvent!();
    }
    
    protected float Speed;
}