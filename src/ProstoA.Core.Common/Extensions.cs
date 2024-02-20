namespace ProstoA.Core;

public static class Extensions
{
    public static TResult GetOrDefault<TResult>(this Maybe<TResult> value)
        => value.TryGet(out var result) ? result : ValueConverter<TResult>.GetDefault();
    
    public static TResult Get<TResult>(
        this Either<TResult, Func<TResult>> value) 
        => value
            .Map(x => x, x => x())
            .GetOrDefault();
    
    
    /// <summary>
    /// Returns the value if is some, otherwise throws an exception.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="InvalidOperationException"></exception>
    public static TResult GetOrThrow<TResult>(this IValueAccessor value)
        => value.TryGet(out TResult result) ? result 
            : throw new InvalidOperationException("The value is None.");
    
    public static TResult GetOrDefault<TResult>(
        this IValueAccessor value,
        Either<TResult, Func<TResult>> defaultValue = default)
        => value.TryGet(out TResult result) ? result : defaultValue.Get();
    
    public static TResult GetOrDefault<TResult>(
        this IValueAccessor value,
        TResult defaultValue)
        => value.GetOrDefault(new Either<TResult, Func<TResult>>(defaultValue));
    
    public static TResult GetOrDefault<TResult>(
        this IValueAccessor value,
        Func<TResult> getDefault)
        => value.GetOrDefault(new Either<TResult, Func<TResult>>(getDefault));


    public static Maybe<TResult> Map<T, TResult>(
        this Maybe<T> value,
        Func<T, TResult> mapper)
        => value.TryGet(out var result) 
            ? Maybe.Some(mapper(result))
            : Maybe.None<TResult>();
    
    public static async Task<Maybe<TResult>> Map<T, TResult>(
        this Maybe<T> value,
        Func<T, Task<TResult>> mapper)
        => value.TryGet(out var result) 
            ? Maybe.Some(await mapper(result))
            : Maybe.None<TResult>();
    
    public static Maybe<TResult> Map<T1, T2, TResult>(
        this Either<T1, T2> value,
        Func<T1, TResult> mapper1,
        Func<T2, TResult> mapper2)
        => value.Map(x => (true, mapper1(x)), x => (true, mapper2(x)));
    
    public static Maybe<TResult> Map<T1, T2, TResult>(
        this Either<T1, T2> value,
        Func<T1, (bool, TResult)> mapper1,
        Func<T2, TResult> mapper2)
        => value.Map(mapper1, x => (true, mapper2(x)));
    
    public static Maybe<TResult> Map<T1, T2, TResult>(
        this Either<T1, T2> value,
        Func<T1, TResult> mapper1,
        Func<T2, (bool, TResult)> mapper2)
        => value.Map(x => (true, mapper1(x)), mapper2);
    
    public static Maybe<TResult> MapT1<T1, T2, TResult>(
        this Either<T1, T2> value,
        Func<T1, TResult> mapper)
        => value.Map(x => (true, mapper(x)), _ => (false, default!));
    
    public static Maybe<TResult> MapT2<T1, T2, TResult>(
        this Either<T1, T2> value,
        Func<T2, TResult> mapper)
        => value.Map(_ => (false, default!), x => (true, mapper(x)));
    
    public static void Do<T1, T2>(
        this Either<T1, T2> value,
        Action<T1>? t1 = default,
        Action<T2>? t2 = default,
        Action? empty = default)
    {
        var action = value.Map<Action>(
            x => (true, () => { t1?.Invoke(x); }),
            x => (true, () => { t2?.Invoke(x); })
        ).TryGet(out var result) ? result : empty;
        
        action?.Invoke();
    }
}