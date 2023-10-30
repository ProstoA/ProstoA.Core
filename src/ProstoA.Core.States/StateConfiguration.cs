namespace ProstoA.Core.States;

public abstract class StateConfiguration<TState> : StateConfiguration<TState>.IStateBuilder where TState : notnull
{
    public interface IBuilder
    {
        IStateBuilder State(TState state);
    }
    
    public interface IStateBuilder : IBuilder
    {
        IStateBuilder OnEnter(Action action);
        IStateBuilder OnExit(Action action);
        
        IStateBuilder OnEnterAsync(Func<Task> action);
        IStateBuilder OnExitAsync(Func<Task> action);
    }
    
    private Dictionary<TState, (Func<Task>? OnEnter, Func<Task>? OnExit)> _actions = new();

    private (TState State, Func<Task>? OnEnter, Func<Task>? OnExit)? _current;

    protected abstract void Configure(IBuilder builder);
    
    public (Func<Task>? OnEnter, Func<Task>? OnExit)? Map(TState state, Func<Func<Task>?, Task>? onEnter, Func<Func<Task>?, Task>? onExit)
    {
        if (_actions.Count == 0)
        {
            Configure(this);
            CompliteState();
        }

        return _actions.TryGetValue(state, out var value) ? (
                onEnter is null ? value.OnEnter : () => onEnter(value.OnEnter),
                onExit is null ? value.OnExit : () => onExit(value.OnExit)
            ) : null;
    }

    private void CompliteState(TState? nextState = default)
    {
        if (_current is not null)
        {
            _actions.Add(_current.Value.State, (_current.Value.OnEnter, _current.Value.OnExit));
        }

        _current = nextState is null ? null : (nextState, null, null);
    }
    
    IStateBuilder IBuilder.State(TState state)
    {
        CompliteState(state);
        return this;
    }

    private Func<Task> WrapAsync(Action action)
    {
        return () =>
        {
            action();
            return Task.CompletedTask;
        };
    } 
    
    IStateBuilder IStateBuilder.OnEnter(Action action)
    {
        if (_current is null || _current.Value.OnEnter is not null)
        {
            throw new InvalidOperationException();
        }
        
        _current = (_current.Value.State, WrapAsync(action), _current.Value.OnExit);

        return this;
    }

    IStateBuilder IStateBuilder.OnExit(Action action)
    {
        if (_current is null || _current.Value.OnExit is not null)
        {
            throw new InvalidOperationException();
        }
        
        _current = (_current.Value.State, _current.Value.OnEnter, WrapAsync(action));

        return this;
    }
    
    IStateBuilder IStateBuilder.OnEnterAsync(Func<Task> action)
    {
        if (_current is null || _current.Value.OnEnter is not null)
        {
            throw new InvalidOperationException();
        }
        
        _current = (_current.Value.State, action, _current.Value.OnExit);

        return this;
    }

    IStateBuilder IStateBuilder.OnExitAsync(Func<Task> action)
    {
        if (_current is null || _current.Value.OnExit is not null)
        {
            throw new InvalidOperationException();
        }
        
        _current = (_current.Value.State, _current.Value.OnEnter, action);

        return this;
    }
}