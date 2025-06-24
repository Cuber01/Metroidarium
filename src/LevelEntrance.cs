using Godot;

namespace Metroidarium;

public partial class LevelEntrance : Area2D
{
    [Export] public string EntranceToLevel;
    [Export] public string EntranceToArea;
    [Export] public int EntranceIndex;

    public bool ignoreEntering = false;

    // To make sure player doesn't end up in an enter-exit-... loop while entering a level
    private void _onBodyExited(Node2D body)
    {
        ignoreEntering = false;
    }
}