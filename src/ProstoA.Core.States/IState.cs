using Microsoft.Extensions.DependencyInjection;

namespace ProstoA.Core.States;

public interface IState
{
    public Task OnEnter(IServiceProvider sp);

    public Task OnExit(IServiceProvider sp);
}

public class State<TState> : IState where TState : notnull
{
    private readonly Lazy<Func<IServiceProvider, (Func<Task>? onEnter, Func<Task>? onExit)>> _configuration;

    private static Func<IServiceProvider, (Func<Task>? onEnter, Func<Task>? onExit)> MakeConfiguration(
        TState state,
        Func<Task>? onEnter = default,
        Func<Task>? onExit = default)
    {
        return sp => GetStateConfiguration(sp)?.Map(state, onEnter, onExit) ?? (onEnter, onExit);
    }

    private static Func<Func<Task>?, Task>? Wrap(Func<Func<Task>, Task>? func)
    {
        return func is null ? null : next => func(next ?? (() => Task.CompletedTask));
    }
    
    private static Func<Task>? Wrap2(Func<Func<Task>, Task>? func)
    {
        return func is null ? null : () => func(() => Task.CompletedTask);
    }

    public State(TState underlyingState, Func<Task>? onEnter = default, Func<Task>? onExit = default)
        : this(underlyingState, MakeConfiguration(underlyingState, onEnter, onExit))
    {
    }
    
    public State(TState underlyingState, Func<IServiceProvider, (Func<Task>? onEnter, Func<Task>? onExit)> configuration)
    {
        UnderlyingState = underlyingState;
        _configuration = new Lazy<Func<IServiceProvider, (Func<Task>? onEnter, Func<Task>? onExit)>>(configuration);
    }

    public TState UnderlyingState { get; }

    public override string? ToString() => UnderlyingState.ToString();
    
    private static StateConfiguration<TState>? GetStateConfiguration(IServiceProvider sp)
    {
        var types = new Lazy<(Type StateType, Type ConfiguratinType)?>(() => 
            typeof(TState)
                .GetCustomAttributesData()
                .Where(x => x.AttributeType.Name == typeof(StateConfigurationAttribute<,>).Name)
                .Select(x => x.AttributeType.GetGenericArguments())
                .Select(x => (x[0], x[1]))
                .SingleOrDefault()
                //.SingleOrDefault(x => x.Length == 2 && x[0] == typeof(TState))
                //?[1]
        ).Value;

        if (types is null)
        {
            return default;
        }

        if (types.Value.StateType != typeof(TState))
        {
            throw new InvalidOperationException("Типы не совпадают");
        }
        
        return ActivatorUtilities.CreateInstance(sp, types.Value.ConfiguratinType) as StateConfiguration<TState>;
    }

    public static implicit operator State<TState>(TState state) => new(state);

    public Task OnEnter(IServiceProvider sp) => _configuration.Value(sp).onEnter?.Invoke() ?? Task.CompletedTask;

    public Task OnExit(IServiceProvider sp) => _configuration.Value(sp).onExit?.Invoke() ?? Task.CompletedTask;
}