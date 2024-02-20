using System.Runtime.CompilerServices;

namespace ProstoA.Core.Operations;

public static class OperationResult
{
    internal static class Context
    {
        public static readonly AsyncLocal<bool> FailResult = new();
    }
    
    /// <summary>
    /// Initializes a new successful instance of the <see cref="OperationResult{TValue}"/> with the given <paramref name="value"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OperationResult<TValue, OperationError> Success<TValue>(TValue value) => new(value);
    
    /// <summary>
    /// Initializes a new failed instance of the <see cref="OperationResult{TValue}"/> with a <see cref="OperationError"/>
    /// with the message <paramref name="errorMessage"/>.
    /// </summary>
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static OperationResult<TValue, OperationError> Failure<TValue>(string errorMessage)
        => new(new OperationError(errorMessage));
    
    public static void Fail() => Context.FailResult.Value = true;
}

[AsyncMethodBuilder(typeof(AsyncOperationResultMethodBuilder<>))]
public readonly record struct OperationResult<TValue>
{
    /*public static OperationResult<TValue> Empty = new();
    
    public static implicit operator TValue?(OperationResult<TValue> result)
    {
        if (result == Empty)
        {
            OperationResult.Fail();
        }
        
        return default;
    }*/


    private readonly Either<TValue, Task<(bool Success, TValue Value)>> _results;
    
    public OperationResult()
    {
    } 
    
    public OperationResult(TValue value)
    {
        _results = new Either<TValue, Task<(bool Success, TValue Value)>>(value);
    }
    
    public OperationResult(Task<(bool Success, TValue Value)> value)
    {
        _results = new Either<TValue, Task<(bool Success, TValue Value)>>(value);
    }
    
    public OperationResult(Task<TValue> value)
    {
        _results = new Either<TValue, Task<(bool Success, TValue Value)>>(Wrap(value));
    }

    private static async Task<(bool Success, TValue Value)> Wrap(Task<TValue> task) => (true, await task);

    public Awaiter GetAwaiter() => new(this);
    
    public static implicit operator OperationResult<TValue>(TValue value) => new(value);
    
    public readonly struct Awaiter : ICriticalNotifyCompletion
    {
        private readonly OperationResult<TValue> _owner;

        internal Awaiter(OperationResult<TValue> owner)
        {
            _owner = owner;
        }

        public bool IsCompleted => !_owner._results.TryGet(out Task<(bool, TValue)>? task) || task!.GetAwaiter().IsCompleted;
        
        public void OnCompleted(Action continuation)
        {
            // этот метод вызывается если IsCompleted == false
            
            if (_owner._results.TryGet(out Task<(bool, TValue)>? task))
            {
                //task!.ContinueWith(_ => continuation);
                task!.GetAwaiter().OnCompleted(continuation);
                return;
            }
            
            throw new InvalidOperationException();
        }

        public void UnsafeOnCompleted(Action continuation)
        {
            // этот метод вызывается если IsCompleted == false
            
            if (_owner._results.TryGet(out Task<(bool, TValue)>? task))
            {
                task!.GetAwaiter().UnsafeOnCompleted(continuation);
                return;
            }

            throw new InvalidOperationException();
        }

        public (bool Succcess, TValue? Value, Exception? Ex) GetResult()
        {
            if (_owner._results.TryGet(out TValue? value))
            {
                // todo: default value processing
                
                return (true, value, default);
            }

            if (_owner._results.TryGet(out Task<(bool Success, TValue Value)>? task))
            {
                if (task!.IsFaulted)
                {
                    return (false, default, task.Exception?.InnerException);
                }
                
                var result = task.GetAwaiter().GetResult();
                return (result.Success, result.Value, default);
            }

            return (false, default, default);
        }
    }
}

public struct OperationResult<TValue, TError>
{
    private readonly Either<TValue, Either<Panic, TError[]>> _value;
    private TError[]? _errors;

    public OperationResult(TValue value)
    {
        _value = Either.Some<TValue, Either<Panic, TError[]>>(value);
    }
    
    public OperationResult(Panic panic)
    {
        _value = Either.Some<TValue, Either<Panic, TError[]>>(panic);
    }
    
    public OperationResult(params TError[] errors)
    {
        _value = Either.Some<TValue, Either<Panic, TError[]>>(errors);
    }

    public bool Success => _value.TryGet(out TValue _);

    // todo: debounce logic
    public TError[] Errors => _errors ??= _value.GetOrDefault(Array.Empty<TError>());

    public static explicit operator TValue(OperationResult<TValue, TError> value) 
        => value._value.GetOrThrow<TValue>();
    
    // public static implicit operator OperationResult<TValue, TError>(TValue value) => new(value);
    // public static implicit operator OperationResult<TValue, TError>(TError error) => new(error);
    // public static implicit operator OperationResult<TValue, TError>(TError[] error) => new(error);
}