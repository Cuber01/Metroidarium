using System.Collections.Generic;
using System.Linq;
using Godot;

namespace Metroidarium;

public interface IState<T>
{
    public void Enter(T entity);
    public void Update(T entity);
    public void Exit(T entity);
}

public class TimedState<T> : IState<T>
{
    public int Time = 0;
    protected int Delay = -1;

    public virtual void Enter(T entity)
    {
        Time = 0;
    }

    public virtual void Update(T entity) {
        Time += 1;
    }
    public virtual void Exit(T entity) { }
    
    public bool TimerCondition()
    {
        return (Time >= Delay);
    }
}


public class StateMachine<T>
{
    private readonly T entity;
    public IState<T> CurrentState { get; private set; }
    
    private List<Transition<T>> localTransitions = new();
    private List<Transition<T>> possibleTransitions = new();
    private List<Transition<T>> globalTransitions = new();

    public StateMachine(T entity, IState<T> initialState)
    {
        this.entity = entity;
        CurrentState = initialState;
        CurrentState.Enter(this.entity);
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

    public void Update()
    {
        Transition<T> transition = GetTransition();
        if (transition != null) {
            SetState(transition.To);
        }
        
        CurrentState.Update(entity);
    }

    private void SetState(IState<T> newState)
    {
        CurrentState.Exit(entity);
        CurrentState = newState;
        CurrentState.Enter(entity);

        calculatePossibleTransitions();
        foreach (Transition<T> transition in localTransitions)
        {
            GD.Print(transition.To.GetType());
        }
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
