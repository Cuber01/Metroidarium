using System.Collections.Generic;
using System.Linq;

namespace Metroidarium;

public interface IState<T> 
{
    void Enter(T entity);
    void Execute(T entity);
    void Exit(T entity);
}

public class StateMachine<T>
{
    private readonly T entity;
    public IState<T> CurrentState { get; private set; }
    private readonly Dictionary<System.Type, IState<T>> states = new();
    
    private List<Transition<T>> localTransitions = new();
    private List<Transition<T>> possibleTransitions = new();
    private List<Transition<T>> globalTransitions = new();

    public StateMachine(T entity, IState<T> initialState)
    {
        this.entity = entity;
        CurrentState = initialState;
        CurrentState.Enter(this.entity);
    }

    public void AddState(IState<T> state) => 
        states[state.GetType()] = state;
    public void AddTransition(IState<T> from, IState<T> to, System.Func<bool> condition) => 
        localTransitions.Add(new Transition<T>(to, condition, from));
    public void AddGlobalTransition(IState<T> to, System.Func<bool> condition) => 
        globalTransitions.Add(new Transition<T>(to, condition, null));

    public void Update()
    {
        Transition<T> transition = GetTransition();
        if (transition != null) {
            SetState(transition.To);
        }
            
        CurrentState.Execute(entity);
    }

    private void SetState(IState<T> newState)
    {
        CurrentState.Exit(entity);
        CurrentState = newState;
        CurrentState.Enter(entity);
        
        possibleTransitions = localTransitions
            .Where(t => t.From == CurrentState.GetType())
            .ToList();
    }

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
