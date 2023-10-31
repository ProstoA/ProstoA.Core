namespace ProstoA.Core.States;

public abstract class StateConfiguration<TState> : StateConfiguration<TState>.IStateBuilder where TState : notnull
{
    public interface IBuilder
    {
        IStateBuilder State(TState state);
    }
    
    public interface IStateBuilder : IBuilder
    {
        IStateBuilder OnBeforeEnter(Action action);
        IStateBuilder OnAfterEnter(Action action);
        IStateBuilder OnBeforeExit(Action action);
        IStateBuilder OnAfterExit(Action action);
        
        IStateBuilder OnBeforeEnterAsync(Func<Task> action);
        IStateBuilder OnAfterEnterAsync(Func<Task> action);
        IStateBuilder OnBeforeExitAsync(Func<Task> action);
        IStateBuilder OnAfterExitAsync(Func<Task> action);
    }
    
    private readonly Dictionary<TState, (Func<Func<Task>?, Task>? OnEnter, Func<Func<Task>?, Task>? OnExit)> _actions = new();

    private (TState State, Func<Func<Task>?, Task> OnEnter, Func<Func<Task>?, Task> OnExit)? _current;

    protected abstract void Configure(IBuilder builder);
    
    public (Func<Task>? OnEnter, Func<Task>? OnExit)? Map(TState state, Func<Task>? onEnter, Func<Task>? onExit)
    {
        if (_actions.Count == 0)
        {
            Configure(this);
            CompliteState();
        }

        return _actions.TryGetValue(state, out var value) ? (
            value.OnEnter is null ? onEnter : () => value.OnEnter(onEnter),
            value.OnExit is null ? onExit : () => value.OnExit(onExit)
        ) : null;
    }

    private void CompliteState(TState? nextState = default)
    {
        
        if (_current is not null)
        {
            _actions.Add(_current.Value.State, (_current.Value.OnEnter, _current.Value.OnExit));
        }

        _current = nextState is null ? null : (
            nextState,
            next => next?.Invoke() ?? Task.CompletedTask,
            next => next?.Invoke() ?? Task.CompletedTask
        );
    }
    
    IStateBuilder IBuilder.State(TState state)
    {
        CompliteState(state);
        return this;
    }
    
    IStateBuilder IStateBuilder.OnBeforeEnter(Action action)
    {
        if (_current is null)
        {
            throw new InvalidOperationException();
        }
        
        var currentOnEnter = _current.Value.OnEnter;
        
        _current = (
            _current.Value.State,
            async next => { action(); await currentOnEnter(next); },
            _current.Value.OnExit
        );

        return this;
    }
    
    IStateBuilder IStateBuilder.OnAfterEnter(Action action)
    {
        if (_current is null)
        {
            throw new InvalidOperationException();
        }
        
        var currentOnEnter = _current.Value.OnEnter;
        
        _current = (
            _current.Value.State,
            async next => { await currentOnEnter(next); action(); },
            _current.Value.OnExit
        );

        return this;
    }

    IStateBuilder IStateBuilder.OnBeforeExit(Action action)
    {
        if (_current is null)
        {
            throw new InvalidOperationException();
        }
        
        var currentOnExit = _current.Value.OnExit;
        
        _current = (
            _current.Value.State,
            _current.Value.OnEnter,
            async next => { action(); await currentOnExit(next); }
        );
        
        return this;
    }
    
    IStateBuilder IStateBuilder.OnAfterExit(Action action)
    {
        if (_current is null)
        {
            throw new InvalidOperationException();
        }
        
        var currentOnExit = _current.Value.OnExit;
        
        _current = (
            _current.Value.State,
            _current.Value.OnEnter,
            async next => { await currentOnExit(next); action(); }
        );
        
        return this;
    }
    
    IStateBuilder IStateBuilder.OnBeforeEnterAsync(Func<Task> action)
    {
        if (_current is null)
        {
            throw new InvalidOperationException();
        }
        
        var currentOnEnter = _current.Value.OnEnter;
        
        _current = (
            _current.Value.State,
            async next => { await action(); await currentOnEnter(next); },
            _current.Value.OnExit
        );

        return this;
    }
    
    IStateBuilder IStateBuilder.OnAfterEnterAsync(Func<Task> action)
    {
        if (_current is null)
        {
            throw new InvalidOperationException();
        }
        
        var currentOnEnter = _current.Value.OnEnter;
        
        _current = (
            _current.Value.State,
            async next => { await currentOnEnter(next); await action(); },
            _current.Value.OnExit
        );

        return this;
    }
    
    IStateBuilder IStateBuilder.OnBeforeExitAsync(Func<Task> action)
    {
        if (_current is null)
        {
            throw new InvalidOperationException();
        }
        
        var currentOnExit = _current.Value.OnExit;
        
        _current = (
            _current.Value.State,
            _current.Value.OnEnter,
            async next => { await action(); await currentOnExit(next); }
        );

        return this;
    }
    
    IStateBuilder IStateBuilder.OnAfterExitAsync(Func<Task> action)
    {
        if (_current is null)
        {
            throw new InvalidOperationException();
        }
        
        var currentOnExit = _current.Value.OnExit;
        
        _current = (
            _current.Value.State,
            _current.Value.OnEnter,
            async next => { await currentOnExit(next); await action(); }
        );

        return this;
    }
}