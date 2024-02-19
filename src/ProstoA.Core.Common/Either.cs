namespace ProstoA.Core;

public readonly record struct Either<T1, T2> : IAccessor<Either<T1, T2>>
{
    public static readonly Either<T1, T2> None = new();
    
    private readonly T1 _value1 = default!;
    private readonly T2 _value2 = default!;
    private readonly byte _index;
    
    public Either(T1 value)
    {
        _index = 1;
        _value1 = value;
    }
    
    public Either(T2 value)
    {
        _index = 2;
        _value2 = value;
    }

    public bool TryGet(out T1 value)
    {
        value = _value1;
        return _index == 1;
    }
    
    public bool TryGet(out T2 value)
    {
        value = _value2;
        return _index == 2;
    }
    
    public Maybe<TResult> Map<TResult>(
        Func<T1, (bool, TResult)> mapper1,
        Func<T2, (bool, TResult)> mapper2) => _index switch
    {
        0 => Maybe<TResult>.None,
        1 => Maybe<TResult>.Wrap(_value1, mapper1),
        2 => Maybe<TResult>.Wrap(_value2, mapper2),
        _ => throw new InvalidOperationException()
    };

    public static implicit operator Either<T1, T2>(T1 result) => new(result);
    public static implicit operator Either<T1, T2>(T2 result) => new(result);

    public static TResult Get<TResult>(Either<T1, T2> container, Func<TResult> getDefault)
    {
        return container
            .Map(ValueConverter<T1>.Convert<TResult>, ValueConverter<T2>.Convert<TResult>)
            .TryGet(out var result) ? result : getDefault();
    }
}