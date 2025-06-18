using Godot;

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
    
    private void _onHurtboxBodyEntered(Node2D body)
    {
        if (body is Bullet bullet)
        {
            bullet.die();
        }
        
        if (body.IsInGroup("Team Baddies"))
        {
            Entity enemy = (Entity)body;
            getHurt(enemy.GetComponent<ContactComponent>().ContactDamageDealt);
        }
    }

    public override void getHurt(int damage)
    {
        base.getHurt(damage);
        OnGotHitEvent!();
    }
    
    protected float Speed;
}