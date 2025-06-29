using System.Collections.Generic;
using Godot;

namespace Metroidarium;

public partial class SnakeBody : Mob
{
    public delegate void GotHitHandler();
    public event GotHitHandler OnGotHitEvent;
    
    public delegate void DiedHandler(int partId);
    public event DiedHandler OnDeathEvent;
    
    public List<SnakeBody> SnakeParts = new List<SnakeBody>();

    protected SnakeBody AheadMe
    {
        get
        {
            for (int i = PartId-1; i >= 0; i--)
            {
                if (SnakeParts[i] != null)
                {
                    return SnakeParts[i];
                }
            }

            return null;
        }
    }

    protected SnakeBody BehindMe
    {
        get
        {
            for (int i = PartId+1; i < SnakeParts.Count; i++)
            {
                if (SnakeParts[i] != null)
                {
                    return SnakeParts[i];
                }
            }

            return null;
        }
    }

    public int PartId = -999;

    public override void _Ready()
    {
        AddComponent(new HealthComponent(this,1, 3f));
        GetComponent<HealthComponent>().InvincibilityEnded += onInvincibilityEnded;
    }
    
    public void setSpeed(float speed)
    {
        this.Speed = speed;
    }

    public void makeInvincible()
    {
        Sprite2D sprite = (Sprite2D)GetNode("Sprite2D");
        ShaderMaterial shader = (ShaderMaterial)sprite.Material;
        shader.SetShaderParameter("run", true);
        GetComponent<HealthComponent>().MakeInvincible();
    }

    private void onInvincibilityEnded()
    {
        Sprite2D sprite = (Sprite2D)GetNode("Sprite2D");
        ShaderMaterial shader = (ShaderMaterial)sprite.Material;
        shader.SetShaderParameter("run", false);
    }
    
    public override void die()
    {
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

}