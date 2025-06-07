using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Metroidarium;

public partial class Enemy : CharacterBody2D
{
    private TileMapLayer FloorTilemap { get; set; }
    private TileMapLayer WallTilemap { get; set; }
    [Export] public Vector2 TileSize { get; set; }

    [Export] public Entity Target {get; set;}
    
    private AStarGrid2D pathFinder;

    public override void _Ready()
    {
        FloorTilemap = GetNode<TileMapLayer>("../Level/Floor");
        WallTilemap = GetNode<TileMapLayer>("../Level/Wall");
        
        initPathFinder();
    }

    public override void _PhysicsProcess(double delta)
    {

        
    }

    private void initPathFinder()
    {
        pathFinder = new AStarGrid2D();
        
        pathFinder.Region = FloorTilemap.GetUsedRect();
        pathFinder.CellSize = FloorTilemap.GetTileSet().TileSize;
        
        pathFinder.Update();
        
        Array<Vector2I> wallCoords = WallTilemap.GetUsedCells();
        foreach (Vector2I wallCoord in wallCoords)
        {
            pathFinder.SetPointSolid(wallCoord);
        }
        
        Vector2I myMapPosition = FloorTilemap.LocalToMap((Vector2I)Position.Round());
        Vector2I targetMapPosition = FloorTilemap.LocalToMap((Vector2I)Target.Position.Round());
    }
}