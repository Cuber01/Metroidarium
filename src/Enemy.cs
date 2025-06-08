using System.Collections.Generic;
using Godot;
using Godot.Collections;

namespace Metroidarium;

public partial class Enemy : Entity
{
    private TileMapLayer FloorTilemap { get; set; }
    private TileMapLayer WallTilemap { get; set; }
    [Export] public Vector2 TileSize { get; set; }

    [Export] public Entity Target {get; set;}
    
    private AStarGrid2D pathFinder;

    private Vector2[] path;
    private int currentCellIndex = 1;
    private int timeTilRecalculate = 100;

    private float speed = 50f;
    
    public override void _Ready()
    {
        FloorTilemap = GetNode<TileMapLayer>("../Level/Floor");
        WallTilemap = GetNode<TileMapLayer>("../Level/Wall");
        
        initPathFinder();
        recalculatePath();
    }

    public override void _PhysicsProcess(double delta)
    {
        // timeTilRecalculate--;
        // if (timeTilRecalculate == 0)
        // {
        //     timeTilRecalculate = 100;
        //     recalculatePath();
        // }
        recalculatePath();
        moveAlongPath();
    }

    private void initPathFinder()
    {
        pathFinder = new AStarGrid2D();
        
        pathFinder.Region = FloorTilemap.GetUsedRect();
        pathFinder.CellSize = FloorTilemap.GetTileSet().TileSize;
        pathFinder.DiagonalMode = AStarGrid2D.DiagonalModeEnum.OnlyIfNoObstacles;
        
        pathFinder.Update();
        
        Array<Vector2I> wallCoords = WallTilemap.GetUsedCells();
        foreach (Vector2I wallCoord in wallCoords)
        {
            pathFinder.SetPointSolid(wallCoord);
        }
    }

    private void moveAlongPath()
    {
        //Vector2 nextCell = path[currentCellIndex];
        //Vector2 nextPoint = new Vector2(nextCell.X*8 + 4, nextCell.Y*8 + 4);
        
        Vector2 direction = (path[currentCellIndex] - Position).Normalized();
        velocity = direction * speed;
        Velocity = velocity;
        MoveAndSlide();
    }
    
    private void recalculatePath()
    {
        // TODO: IF WE PUT THE HITBOX SO THAT THE CENTER IS AT 0,0 THIS ROUNDING BREAKS AND CHARACTER GETS STUCK ON CORNERS
        Vector2I myMapPosition = FloorTilemap.LocalToMap((Vector2I)Position.Round());
        GD.Print(myMapPosition);
        Vector2I targetMapPosition = FloorTilemap.LocalToMap((Vector2I)Target.Position.Round());
        path = pathFinder.GetPointPath(myMapPosition, targetMapPosition);
    }
}