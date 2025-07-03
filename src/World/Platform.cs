using System.Collections.Generic;
using Godot;

namespace Metroidarium;

public partial class Platform : Node2D
{
    enum Breaks
    {
        Never,
        TouchedByPlayer,
        OnTimer
    }

    [Export] Breaks BreakType { get; set; } = Breaks.Never;
    [Export] private float delayTilBreak = 5;
    [Export] private float delayTilRebuild = 5;
    [Export] private bool destructionPermanent = false;

    private Sprite2D sprite;
    private Area2D area;
    
    private bool touchedByPlayer = false;
    private bool on = true;
    private float timer = 0;
    
    protected List<Mob> CarriedMobs = new List<Mob>();

    public override void _Ready()
    {
        sprite = GetNode<Sprite2D>("Path2D/AnimatableBody2D/Sprite2D");
        area = GetNode<Area2D>("Path2D/AnimatableBody2D/Area2D");
    }
    
    public override void _PhysicsProcess(double delta)
    {
        if (BreakType == Breaks.OnTimer || (BreakType == Breaks.TouchedByPlayer && touchedByPlayer))
        {
            updateTimer((float)delta);
        }
    }
    
    private void updateTimer(float delta)
    {
        timer += delta;
        if (timer >= (on ? delayTilRebuild : delayTilBreak))
        {
            changeState(!on);
        }
    }

    private void changeState(bool newOnState)
    {
        timer = 0;
        on = newOnState;
        // Do not optimize out this if statement, we will add animations later that might require it
        if (newOnState)
        {
            sprite.Visible = true;
            area.SetDeferred(Area2D.PropertyName.Monitoring, true);
            touchedByPlayer = false;
        }
        else
        {
            if (destructionPermanent)
            {
                CallDeferred(Node.MethodName.QueueFree);
                return;
            }

            sprite.Visible = false;
            area.SetDeferred(Area2D.PropertyName.Monitoring, false);
        }
    }
    
    private void _onMobEntered(PhysicsBody2D body)
    {
        Mob mob = (Mob)body;
        CarriedMobs.Add(mob);
        mob.CanFall = false;
        mob.IsOnPlatform = true;

        if (body is SnakeHead)
        {
            touchedByPlayer = true;
        }
    }
    
    private void _onMobExited(PhysicsBody2D body)
    {
        Mob mob = (Mob)body;
        CarriedMobs.Remove(mob);
        mob.CanFall = true;
        mob.IsOnPlatform = false;
    }
}