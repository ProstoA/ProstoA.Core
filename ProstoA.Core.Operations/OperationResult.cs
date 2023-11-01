namespace ProstoA.Core.Operations;

public struct OperationResult<TValue, TError>
{
    private readonly Either<TValue, TError[]> _results;
    private TError[]? _errors;

    public OperationResult(TValue value)
    {
        _results = new Either<TValue, TError[]>(value);
    }
    
    public OperationResult(params TError[] errors)
    {
        _results = new Either<TValue, TError[]>(errors!);
    }

    public bool Success => _results.TryGetResult(out TValue _);
    
    public TError[] Errors => _errors ??= _results.Get(Array.Empty<TError>);

    public static implicit operator TValue(OperationResult<TValue, TError> result)
        => result._results.Get<TValue>(() => throw new InvalidOperationException());
    
    public static implicit operator OperationResult<TValue, TError>(TValue value) => new(value);
    public static implicit operator OperationResult<TValue, TError>(TError error) => new(error);
    public static implicit operator OperationResult<TValue, TError>(TError[] error) => new(error);
}