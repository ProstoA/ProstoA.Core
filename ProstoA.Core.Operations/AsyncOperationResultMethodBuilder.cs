using System.Runtime.CompilerServices;

namespace ProstoA.Core.Operations;

public struct AsyncOperationResultMethodBuilder<TResult>
{
    // AsyncTaskMethodBuilder<TResult>
    // AsyncValueTaskMethodBuilder<TResult>
    
    // https://habr.com/ru/articles/470830/
    // https://habr.com/ru/articles/732738/
    // https://learn.microsoft.com/ru-ru/dotnet/csharp/language-reference/attributes/general
    
    // other
    // https://blog.i3arnon.com/2018/01/02/task-enumerable-awaiter/
    // https://blog.stephencleary.com/2014/12/a-tour-of-task-part-6-results.html
    // https://dev.to/nikiforovall/awaitable-awaiter-pattern-and-logical-micro-threading-in-c-2701
    // https://codeblog.jonskeet.uk/2011/05/17/eduasync-part-5-making-task-lt-t-gt-awaitable/
    
    private AsyncTaskMethodBuilder<(bool Success, TResult Value)> _taskBuilder = AsyncTaskMethodBuilder<(bool Success, TResult Value)>.Create();
    private OperationResult<TResult>? _task;

    public AsyncOperationResultMethodBuilder()
    {
    }

    public static AsyncOperationResultMethodBuilder<TResult> Create() => new();
    
    public void Start<TStateMachine>(ref TStateMachine stateMachine) where TStateMachine : IAsyncStateMachine
    {
        _taskBuilder.Start(ref stateMachine);
        
        // var previous = Thread.CurrentThread.ExecutionContext; // [ThreadStatic] field
        // try
        // {
        //     stateMachine.MoveNext();
        // }
        // finally
        // {
        //     if (previous is not null)
        //     {
        //         ExecutionContext.Restore(previous); // internal helper
        //     }
        // }
    }
    
    public void SetStateMachine(IAsyncStateMachine stateMachine)
    {
        _taskBuilder.SetStateMachine(stateMachine);
    }
    
    public void SetException(Exception exception)
    {
        _taskBuilder.SetException(exception);
    }
    
    public void SetResult(TResult result)
    {
        _taskBuilder.SetResult((!OperationResult.Context.FailResult.Value, result));
    }
    
    public void AwaitOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : INotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        _taskBuilder.AwaitOnCompleted(ref awaiter, ref stateMachine);
    }
    
    public void AwaitUnsafeOnCompleted<TAwaiter, TStateMachine>(ref TAwaiter awaiter, ref TStateMachine stateMachine)
        where TAwaiter : ICriticalNotifyCompletion
        where TStateMachine : IAsyncStateMachine
    {
        _taskBuilder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
    }

    public OperationResult<TResult> Task => _task ??= new OperationResult<TResult>(_taskBuilder.Task);
}