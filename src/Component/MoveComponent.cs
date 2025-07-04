using System.Collections.Generic;
using System.IO;
using Godot;
using Godot.Collections;
// ReSharper disable Godot.MissingParameterlessConstructor
namespace Metroidarium;

public partial class MoveComponent : Component
{
    protected CharacterBody2D Actor;
    protected float Speed;

    public MoveComponent(CharacterBody2D actor, float speed)
    {
        this.Actor = actor;
        this.Speed = speed;
    }
}

public partial class AStarMoveComponent : MoveComponent
{
    private AStarGrid2D pathFinder;
    private TileMapLayer passable;
    private TileMapLayer unpassable;
    
    private ToPointMoveComponent mover;
    private Vector2[] path;
    private int currentPathIndex = 0;
    private const int RecalculateCounterMax = 10;
    private int recalculateCounter = RecalculateCounterMax;
    
    private AStarGrid2D.DiagonalModeEnum diagonalMode = AStarGrid2D.DiagonalModeEnum.OnlyIfNoObstacles; 
    
    public AStarMoveComponent(Mob actor, float speed, Vector2 targetPosition, TileMapLayer passable, TileMapLayer unpassable) : base(actor, speed)
    {
        // Init fields
        this.Speed = speed;
        this.Actor = actor;
        this.passable = passable;
        this.unpassable = unpassable;
        
        // Init AStarGrid2D
        pathFinder = new AStarGrid2D();
        
        pathFinder.Region = passable.GetUsedRect();
        pathFinder.CellSize = passable.GetTileSet().TileSize;
        pathFinder.DiagonalMode = diagonalMode;
        
        pathFinder.Update();
        
        Array<Vector2I> wallCoords = unpassable.GetUsedCells();
        foreach (Vector2I wallCoord in wallCoords)
        {
            pathFinder.SetPointSolid(wallCoord);
        }
        
        calculatePath(targetPosition);
        
        // Init mover
        mover = new ToPointMoveComponent(actor, path[currentPathIndex], speed, false);
        
        mover.OnReachedEvent += reachedPathPoint;
    }

    public void update(Vector2 target)
    {
        recalculateCounter--;
        if (recalculateCounter <= 0)
        {
            recalculateCounter = RecalculateCounterMax;
            calculatePath(target);
            currentPathIndex = 0;
            mover.changePoint(path[0]);
        }
        mover.update();
    }
    
    private void calculatePath(Vector2 targetPosition)
    {
        // TODO: IF WE PUT THE HITBOX SO THAT THE CENTER IS AT 0,0 THIS ROUNDING BREAKS AND CHARACTER GETS STUCK ON CORNERS
        Vector2I myMapPosition = passable.LocalToMap((Vector2I)Actor.Position.Round());
        Vector2I targetMapPosition = passable.LocalToMap((Vector2I)targetPosition.Round());
        path = pathFinder.GetPointPath(myMapPosition, targetMapPosition);
        debugDrawPath();
    }

    private void debugDrawPath()
    {
        Line2D line = Actor.GetNode<Line2D>("Line2D");
        
        line.ClearPoints();
        foreach (Vector2I point in path)
        {
            line.AddPoint(point);
        }
    }

    private void reachedPathPoint()
    {
        currentPathIndex++;
        if (currentPathIndex >= path.Length)
        {
            recalculateCounter = 0;
        }
        else
        {
            mover.changePoint(path[currentPathIndex]);
        }
        
    }
}
public class ToPointMoveComponent : MoveComponent
{

    public delegate void ReachedPointEventHandler();
    public event ReachedPointEventHandler OnReachedEvent;
    
    protected Vector2 Point;
    protected bool StopWhenReached = false;
    
    private DirectionalMoveComponent mover;

    // TODO this shouldnt work on an error margin, i think
    private static readonly Vector2 ErrorMargin = new Vector2(2,8);

    public ToPointMoveComponent(Mob actor, Vector2 point, float speed, bool stopWhenReached) : base(actor, speed)
    {
        this.Point = point;
        this.StopWhenReached = stopWhenReached;
        
        mover = new DirectionalMoveComponent(actor, calculateDirection(actor.Position, point), speed);
    }

    public static Vector2 calculateDirection(Vector2 myPos, Vector2 targetPos)
    {
        return (targetPos - myPos).Normalized();
    }

    public void changePoint(Vector2 point)
    {
        this.Point = point;
        mover.changeDirection(calculateDirection(Actor.Position, point));
    }
    
    public void update()
    {
        if (isAtPoint(Actor.Position, Point))
        {
            OnReachedEvent();
        }
        mover.update();
    }

    private bool isAtPoint(Vector2 myPosition, Vector2 pointPosition)
    {
        return (myPosition.X >= pointPosition.X-ErrorMargin.X && myPosition.Y >= pointPosition.Y-ErrorMargin.X && myPosition.X <= pointPosition.X + ErrorMargin.Y && myPosition.Y <= pointPosition.Y + ErrorMargin.Y);
    }
}

public partial class DirectionalMoveComponent : MoveComponent
{
    private Vector2 direction;
    public DirectionalMoveComponent(CharacterBody2D actor, Vector2 direction, float speed) : base(actor, speed)
    {
        this.Speed = speed;
        this.Actor = actor;
        this.direction = direction;
    }
    
    public void changeDirection(Vector2 direction)
    {
        this.direction = direction;
    }
    
    public void update()
    {
        Actor.Velocity = direction * Speed;
    }

    public void update(float customSpeed)
    {
        Actor.Velocity = direction * customSpeed;
    }
}

public partial class TweenToPointComponent : ToPointMoveComponent
{
    private DirectionalMoveComponent mover;

    private Tween.EaseType ease;
    private Tween.TransitionType transition;
    private float endSpeed;
    private float speedDelta;
    private float currentTweenTime = 0f;
    
    public TweenToPointComponent(Mob actor, Vector2 point, bool stopWhenReached, float startSpeed, float speedDelta,
        float endSpeed, Tween.EaseType ease, Tween.TransitionType transition) : base(actor, point, startSpeed, stopWhenReached)
    {
        this.ease = ease;
        this.transition = transition;
        this.endSpeed = endSpeed;
        this.speedDelta = speedDelta;
        mover = new DirectionalMoveComponent(actor, point.Normalized(), Speed);
    }

    public void update()
    {
        if ( !(StopWhenReached && Actor.Position.IsEqualApprox(Point)) )
        {
            float currentSpeed = (float)Tween.InterpolateValue(Speed, speedDelta, currentTweenTime, (Speed-endSpeed)/speedDelta, transition, ease);;
            mover.update(currentSpeed);  
        }
    }
    
}


