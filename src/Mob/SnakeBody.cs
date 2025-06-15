namespace Metroidarium;

public partial class SnakeBody : Mob
{
    public delegate void GotHitHandler();
    public event GotHitHandler OnGotHitEvent;
    
    public delegate void DiedHandler(int partId);
    public event DiedHandler OnDeathEvent;
    
    protected SnakeBody aheadMe = null;
    public SnakeTail behindMe = null;

    public int partId = -999;
    
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
        if (aheadMe != null)
        {
            aheadMe.behindMe = behindMe;
        }

        if (this is not SnakeHead)
        {
            OnDeathEvent!(partId);    
        }
        
        base.die();
    }

    public override void getHurt(int damage)
    {
        base.getHurt(damage);
        OnGotHitEvent!();
    }
    
    protected float Speed;
}