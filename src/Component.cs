using System.Collections.Generic;
using Godot;
using Godot.Collections;
// ReSharper disable Godot.MissingParameterlessConstructor
namespace Metroidarium;

public partial class Component : Node2D { }

public partial class MoveComponent : Component
{
    protected Entity Actor;
    protected float Speed;

    public MoveComponent(Entity actor, float speed)
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
    private const int RecalculateCounterMax = 200;
    private int recalculateCounter = RecalculateCounterMax;
    
    private AStarGrid2D.DiagonalModeEnum diagonalMode = AStarGrid2D.DiagonalModeEnum.OnlyIfNoObstacles; 
    
    public AStarMoveComponent(Entity actor, float speed, Vector2 targetPosition, TileMapLayer passable, TileMapLayer unpassable) : base(actor, speed)
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
        mover = new ToPointMoveComponent(actor, targetPosition, speed, false);
        
        mover.ReachedPoint += reachedPathPoint;
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
    }

    private void reachedPathPoint()
    {
        currentPathIndex++;
        mover.changePoint(path[currentPathIndex]);
    }
}
public partial class ToPointMoveComponent : MoveComponent
{
    [Signal]
    public delegate void ReachedPointEventHandler();
    
    private DirectionalMoveComponent mover;
    protected Vector2 Point;
    protected bool StopWhenReached = false;
    private bool reached = false;
    
    public ToPointMoveComponent(Entity actor, Vector2 point, float speed, bool stopWhenReached) : base(actor, speed)
    {
        this.Point = point;
        this.StopWhenReached = stopWhenReached;
        mover = new DirectionalMoveComponent(actor, point.Normalized(), speed);
    }

    public void changePoint(Vector2 point)
    {
        this.Point = point;
        mover.changeDirection(point.Normalized());
    }
    
    public void update()
    {
        if (Actor.Position.IsEqualApprox(Point))
        {
            reached = true;
            EmitSignal(SignalName.ReachedPoint);
        }
    }
}

public partial class DirectionalMoveComponent : MoveComponent
{
    private Vector2 direction;
    public DirectionalMoveComponent(Entity actor, Vector2 direction, float speed) : base(actor, speed)
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
        Actor.velocity = direction * Speed;
    }

    public void update(float customSpeed)
    {
        Actor.velocity = direction * customSpeed;
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
    
    public TweenToPointComponent(Entity actor, Vector2 point, bool stopWhenReached, float startSpeed, float speedDelta,
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


