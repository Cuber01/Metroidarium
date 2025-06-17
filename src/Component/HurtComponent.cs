namespace Metroidarium;

public class HurtComponent : Component
{
    public int DealDamage = 0;

    public HurtComponent(int damage)
    {
        this.DealDamage = damage;
    }
}