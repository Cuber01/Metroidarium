using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Metroidarium;

public interface IState<T>
{
    public void Enter(T actor);
    public void Update(T actor, float dt);
    public void Exit(T actor);
}

public class TimedState<T> : IState<T>
{
    public float Time = 0;
    protected float Delay = -1;

    public virtual void Enter(T actor)
    {
        Time = 0;
    }

    public virtual void Update(T actor, float dt) {
        Time += dt;
    }
    public virtual void Exit(T actor) { }
    
    public bool TimerCondition()
    {
        return (Time >= Delay);
    }
}


public class StateMachine<T>
{
    private readonly T actor;
    public IState<T> CurrentState { get; private set; }
    
    private List<Transition<T>> localTransitions = new();
    private List<Transition<T>> possibleTransitions = new();
    private List<Transition<T>> globalTransitions = new();

    public StateMachine(T actor, IState<T> initialState)
    {
        this.actor = actor;
        CurrentState = initialState;
        CurrentState.Enter(this.actor);
    }

    public void AddTransition(IState<T> from, IState<T> to, System.Func<bool> condition)
    {
        Transition<T> newTransition = new Transition<T>(to, condition, from); 
        localTransitions.Add(newTransition);
        if (from == CurrentState)
        {
            possibleTransitions.Add(newTransition);
        }
    }
        
    public void AddGlobalTransition(IState<T> to, System.Func<bool> condition) => 
        globalTransitions.Add(new Transition<T>(to, condition, null));

    public void Update(float dt)
    {
        Transition<T> transition = GetTransition();
        if (transition != null) {
            SetState(transition.To);
        }
        
        CurrentState.Update(actor, dt);
    }

    private void SetState(IState<T> newState)
    {
        CurrentState.Exit(actor);
        CurrentState = newState;
        CurrentState.Enter(actor);

        calculatePossibleTransitions();
        // foreach (Transition<T> transition in localTransitions)
        // {
        //     GD.Print(transition.To.GetType());
        // }
    }
    
    private void calculatePossibleTransitions() =>  possibleTransitions = localTransitions
        .Where(t => t.From == CurrentState)
        .ToList();

    private Transition<T> GetTransition()
    {
        // Search global transitions
        foreach (var transition in globalTransitions)
        {
            if (transition.Condition())
                return transition;
        }


        // Else search normal transitions
        return possibleTransitions.FirstOrDefault(t => t.Condition(), null);
    }

    private class Transition<U>
    {
        public readonly IState<U> From;
        public readonly IState<U> To;
        public readonly System.Func<bool> Condition;

        // If from=null it's a global transition
        public Transition(IState<U> to, System.Func<bool> condition, IState<U> from)
        {
            To = to;
            From = from;
            Condition = condition;
        }
    }
}
