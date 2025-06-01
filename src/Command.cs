using System;
using Godot;
using static Godot.Mathf;

namespace Metroidarium;

public partial class Command : Node2D
{
    public virtual void Execute(Entity actor, Object input = null)
    {
        
    }
}

public partial class MoveCommand : Command
{
    public override void Execute(Entity actor, Object input = null)
    {
        float direction = (float)input!;
        actor.velocity.X = direction * actor.Speed;
    }
}

public partial class JumpCommand : Command
{
    public override void Execute(Entity actor, Object input = null)
    {
        actor.velocity.Y = actor.JumpVelocity;
    }
}