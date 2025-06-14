namespace Metroidarium;

public class HealthComponent : Component
{
    private Mob actor;
    private int health;
    private int maxHealth;

    private bool invincible = false;
    private int invincibilityTime = 0;
    private int invincibilityCounter = 0;

    public HealthComponent(Mob actor, int health, int invincibilityTime=0)
    {
        this.actor = actor;
        this.health = health;
        this.maxHealth = health;
        this.invincibilityTime = invincibilityTime;
    }

    public void update()
    {
        if (invincibilityCounter > 0)
        {
            invincibilityCounter--;
            if (invincibilityCounter == 0)
            {
                invincible = false;
            }
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
        invincibilityCounter = invincibilityTime;
    }
}