namespace Metroidarium;

public class HealthComponent : Component
{
    public delegate void InvincibilityEndedEventHandler();
    public event InvincibilityEndedEventHandler InvincibilityEnded;
    
    private Mob actor;
    private int health;
    private int maxHealth;

    private bool invincible = false;
    private float invincibilityTime = 5f;
    private float invincibilityDelay = 0;

    public HealthComponent(Mob actor, int health, float invincibilityTime=0)
    {
        this.actor = actor;
        this.health = health;
        this.maxHealth = health;
        this.invincibilityTime = invincibilityTime;
    }

    public void updateInvincibility(float dt)
    {
        if (!invincible) return;
        
        invincibilityDelay -= dt;
        if (invincibilityDelay <= 0)
        {
            invincible = false;
            InvincibilityEnded?.Invoke();
        }
    }

    public void changeHealth(int amount)
    {
        if (invincible && amount < 0)
        {
            return;
        }
        
        health += amount;
        if (health <= 0)
        {
            actor.die();
        }
        else if (invincibilityTime > 0)
        {
            MakeInvincible();
        }
    }

    public void MakeInvincible()
    {
        invincible = true;
        invincibilityDelay = invincibilityTime;
    }
}