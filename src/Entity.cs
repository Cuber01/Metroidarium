using Godot;

namespace Metroidarium;

public partial class Entity : CharacterBody2D
{
    public readonly float Speed = 300.0f;
    // ReSharper disable once InconsistentNaming
    public Vector2 velocity = Vector2.Zero;
    public readonly float JumpVelocity = -400.0f;
}