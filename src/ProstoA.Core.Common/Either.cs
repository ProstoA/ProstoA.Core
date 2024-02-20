using System.Runtime.CompilerServices;

namespace ProstoA.Core;

public static class Either
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Either<T1, T2> Some<T1, T2>(T1 value) => new(value);
    
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Either<T1, T2> Some<T1, T2>(T2 value) => new(value);

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Either<T1, T2> None<T1, T2>() => Either<T1, T2>.None;
}

public readonly record struct Either<T1, T2> :
    IValueContainer<T1, T2, Either<T1, T2>>,
    IAccessor<Either<T1, T2>>
{
    public static Either<T1, T2> None => default;

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

    public override string ToString() => this
        .Map(x => $"{x}", x => $"{x}")
        .GetOrDefault("None");

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
    
    public (bool, TResult) Get<TResult>()
    {
        return _index switch
        {
            0 => default,
            1 => ValueConverter<T1>.Convert<TResult>(_value1),
            2 => ValueConverter<T2>.Convert<TResult>(_value2),
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public static implicit operator Either<T1, T2>(T1 result) => new(result);
    public static implicit operator Either<T1, T2>(T2 result) => new(result);
    
    public Maybe<TResult> Map<TResult>(
        Func<T1, (bool, TResult)> mapper1,
        Func<T2, (bool, TResult)> mapper2) => _index switch
    {
        0 => Maybe<TResult>.None,
        1 => Maybe<TResult>.Wrap(_value1, mapper1),
        2 => Maybe<TResult>.Wrap(_value2, mapper2),
        _ => throw new ArgumentOutOfRangeException()
    };

    public static TResult Get<TResult>(Either<T1, T2> container, Either<TResult, Func<TResult>> defaultValue)
    {
        return container
            .Map(ValueConverter<T1>.Convert<TResult>, ValueConverter<T2>.Convert<TResult>)
            .GetOrDefault(defaultValue);
    }
}