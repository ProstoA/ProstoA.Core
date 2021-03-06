using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;
using ProstoA.Core.Operations;

namespace ProstoA.Core.States.Tests;

public class UnitTest1
{
    [Fact]
    public void T3()
    {
        var t = new Either<string, int>(5);

        Assert.True(t.TryGetResult(out int _));
        
        t.Do(
            s => Console.WriteLine(s),
            ii => Console.WriteLine(ii),
            () => Console.WriteLine("empty")
        );

        t.Do(one: Console.WriteLine);

        Assert.Equal("123", t.GetOrDefault("123"));
        Assert.Null(t.GetOrDefault<string?>(default));
        Assert.Null(t.GetOrDefault<long?>(default));
    }

    [Fact]
    public void T4()
    {
        var result = new OperationResult<int, string>(5);
        Assert.True(result.Success);
        Assert.Equal(5, (int)result);
        Assert.Empty(result.Errors);

        var err = new OperationResult<int, string>("error");
        Assert.False(err.Success);
        Assert.Throws<InvalidOperationException>(() => (int)err);
        Assert.Single(err.Errors);
        Assert.Equal("error", err.Errors[0]);
        Assert.Same(err.Errors, err.Errors);
        
        var errs = new OperationResult<int, string>("error1", "error2");
        Assert.False(errs.Success);
        Assert.Equal(2, errs.Errors.Length);
        Assert.Equal("error1", errs.Errors[0]);
        Assert.Equal("error2", errs.Errors[1]);

        var res = new Either<int, Either<long, string>>(10L);
        Assert.Equal(10f, res.GetOrDefault(10f));
    }

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

        var stateA = new State<MyStructState>(MyStructState.A, onEnter: async () => mock.LogInformation("Inline"), onExit: async () => mock.LogInformation("Inline"));
        stateA.OnEnter(sp);
        stateA.OnExit(sp);
        
        State<MyStructState> stateB = MyStructState.B;
        stateB.OnEnter(sp);
        stateB.OnExit(sp);
        
        State<MyStructState> stateC = MyStructState.C;
        stateC.OnEnter(sp);
        stateC.OnExit(sp);
        
        mock.Received(9).Log(LogLevel.Information, 0, Arg.Any<object>(), null, Arg.Any<Func<object, Exception?, string>>());
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
                .OnAfterEnter(StateAEnter)
                .OnBeforeExitAsync(StateAExitAsync)
            
            .State(MyState.B)
                .OnAfterEnter(StateBEnter)
            
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
                .OnBeforeEnter(StateAEnter)
                .OnBeforeEnter(StateAEnter2)
                .OnAfterEnter(StateAEnter)
                .OnAfterEnter(StateAEnter2)
                .OnAfterExitAsync(StateAExitAsync)
                .OnAfterExitAsync(StateAExitAsync)
            
            .State(MyStructState.B)
                .OnAfterEnter(StateBEnter)
            
            ;
    }

    private void StateAEnter()
    {
        _logger.LogInformation("State {state} Enter", "A");
    }
    
    private void StateAEnter2()
    {
        _logger.LogInformation("State {state} Enter2", "A");
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