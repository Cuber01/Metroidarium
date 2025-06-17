namespace Metroidarium;

public partial class SnakeBody : Mob
{
    public delegate void GotHitHandler();
    public event GotHitHandler OnGotHitEvent;
    
    public delegate void DiedHandler(int partId);
    public event DiedHandler OnDeathEvent;
    
    protected SnakeBody AheadMe = null;
    public SnakeTail BehindMe = null;

    public int PartId = -999;
    
    public void setSpeed(float speed)
    {
        this.Speed = speed;
    }

    public void makeInvincible()
    {
        GetComponent<HealthComponent>().MakeInvincible();
    }
    
    public override void die()
    {
        if (BehindMe != null)
        {
            BehindMe.AheadMe = AheadMe;
        }
        if (AheadMe != null)
        {
            AheadMe.BehindMe = BehindMe;
        }

        if (this is not SnakeHead)
        {
            OnDeathEvent!(PartId);    
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