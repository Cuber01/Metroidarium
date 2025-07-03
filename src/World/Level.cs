using Godot;

namespace Metroidarium;

public partial class Level : Node2D
{
    private GameManager game;
    
    public override void _Ready()
    {
        game = GetParent<GameManager>();
        Name = "Level";
        
        Node2D entrances = GetNode<Node2D>("Entrances");
        foreach (var node in entrances.GetChildren())
        {
            var entrance = (LevelEntrance)node;
            entrance.BodyEntered += body => _onLevelExit(body, entrance);
        }
    }

    private void _onLevelExit(Node2D body, LevelEntrance entrance)
    {
        if (!entrance.ignoreEntering)
        {
            PackedScene newLevel = game.Levels[entrance.EntranceToLevel];
            Level levelInstance = (Level)newLevel.Instantiate();
            game.CallDeferred("add_child", levelInstance);

            // Reparent player to new level node
            SnakeHead player = (SnakeHead)body;            
            player.callMethodOnSnake(part => part.Reparent(levelInstance));
            
            levelInstance.Enter(entrance.EntranceIndex, player);
            CallDeferred("queue_free");
        }
    }

    public void Enter(int entranceIndex, SnakeHead player)
    {
        
        // Set player at entrance
        Node2D entrances = GetNode<Node2D>("Entrances");
        foreach (var node in entrances.GetChildren())
        {
            var entrance = (LevelEntrance)node;
            if (entrance.EntranceIndex == entranceIndex)
            {
                player.GlobalPosition = entrance.GetNode<Node2D>("SpawnPoint").GlobalPosition;
                player.callMethodOnTail(tail => tail.resetPosition());
            }
        }
    }
}