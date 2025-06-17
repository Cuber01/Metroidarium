namespace Metroidarium;

public class ContactComponent : Component
{
    public readonly int ContactDamageDealt = 0;

    public ContactComponent(int contactDamageDealt)
    {
        this.ContactDamageDealt = contactDamageDealt;
    }
}