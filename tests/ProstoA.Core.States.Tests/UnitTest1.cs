using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

namespace ProstoA.Core.States.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        var mock = Substitute.For<ILogger<MyStateConfiguration>>();
        
        var sp = new ServiceCollection()
            .AddTransient<ILogger<MyStateConfiguration>>(_ => mock)
            .BuildServiceProvider();
        
        State<MyState> stateA = MyState.A;
        stateA.OnEnter(sp);
        stateA.OnExit(sp);
        
        State<MyState> stateB = MyState.B;
        stateB.OnEnter(sp);
        stateB.OnExit(sp);
        
        State<MyState> stateC = MyState.C;
        stateC.OnEnter(sp);
        stateC.OnExit(sp);
        
        mock.Received(3).Log(LogLevel.Information, 0, Arg.Any<object>(), null, Arg.Any<Func<object, Exception?, string>>());
    }
    
    [Fact]
    public void Test2()
    {
        var mock = Substitute.For<ILogger<MyStateConfiguration>>();
        
        var sp = new ServiceCollection()
            .AddTransient<ILogger<MyStateConfiguration>>(_ => mock)
            .BuildServiceProvider();

        var stateA = new State<MyStructState>(MyStructState.A, onEnter: _ => Task.CompletedTask);
        stateA.OnEnter(sp);
        stateA.OnExit(sp);
        
        State<MyStructState> stateB = MyStructState.B;
        stateB.OnEnter(sp);
        stateB.OnExit(sp);
        
        State<MyStructState> stateC = MyStructState.C;
        stateC.OnEnter(sp);
        stateC.OnExit(sp);
        
        mock.Received(2).Log(LogLevel.Information, 0, Arg.Any<object>(), null, Arg.Any<Func<object, Exception?, string>>());
    }
}

[StateConfiguration<MyState, MyStateConfiguration>]
public enum MyState
{
    A,
    B,
    C
}

[StateConfiguration<MyStructState, MyStructStateConfiguration>]
//[StateConfiguration<MyState, MyStateConfiguration>]
public struct MyStructState
{
    public static readonly MyStructState A = new("A");
    public static readonly MyStructState B = new("B");
    public static readonly MyStructState C = new("C");
    
    public MyStructState(string key)
    {
        Key = key;
    }
    
    public string Key { get; }
}

public class MyStateConfiguration : StateConfiguration<MyState>
{
    private readonly ILogger<MyStateConfiguration> _logger;

    public MyStateConfiguration(ILogger<MyStateConfiguration> logger)
    {
        _logger = logger;
    }
    
    protected override void Configure(IBuilder builder)
    {
        builder
            .State(MyState.A)
                .OnEnter(StateAEnter)
                .OnExitAsync(StateAExitAsync)
            
            .State(MyState.B)
                .OnEnter(StateBEnter)
            
            ;
    }

    private void StateAEnter()
    {
        _logger.LogInformation("State {state} Enter", "A");
    }
    
    private async Task StateAExitAsync()
    {
        _logger.LogInformation("State {state} Exit", "A");
    }
    
    private void StateBEnter()
    {
        _logger.LogInformation("State {state} Enter", "B");
    }
}

public class MyStructStateConfiguration : StateConfiguration<MyStructState>
{
    private readonly ILogger<MyStateConfiguration> _logger;

    public MyStructStateConfiguration(ILogger<MyStateConfiguration> logger)
    {
        _logger = logger;
    }
    
    protected override void Configure(IBuilder builder)
    {
        builder
            .State(MyStructState.A)
            .OnEnter(StateAEnter)
            .OnExitAsync(StateAExitAsync)
            
            .State(MyStructState.B)
            .OnEnter(StateBEnter)
            
            ;
    }

    private void StateAEnter()
    {
        _logger.LogInformation("State {state} Enter", "A");
    }
    
    private async Task StateAExitAsync()
    {
        _logger.LogInformation("State {state} Exit", "A");
    }
    
    private void StateBEnter()
    {
        _logger.LogInformation("State {state} Enter", "B");
    }
}