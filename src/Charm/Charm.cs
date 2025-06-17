using System;
using System.Collections.Generic;
using Godot;

namespace Metroidarium;

public class Charm
{
    protected SnakeHead Player;
    
    public virtual void Init() { }
    
    public virtual void Update() { }
    
    public virtual void Destroy() { }
    
    protected virtual void activate() { }
    
    protected virtual void deactivate() { }
}