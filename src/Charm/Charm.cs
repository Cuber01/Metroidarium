using System;
using System.Collections.Generic;
using Godot;

namespace Metroidarium;

public class Charm(SnakeHead player, SnakeTail slot)
{
    protected SnakeHead Player = player;
    protected SnakeTail Slot = slot;
    
    public virtual void Update(float dt) { }
    
    public virtual void Equip(params Object[] parameters) { }
    
    public virtual void Unequip() { }
    
    // For when the charm should start doing stuff
    protected virtual void activate() { }
    
    // For when the charm should stop doing stuff
    protected virtual void deactivate() { }
}