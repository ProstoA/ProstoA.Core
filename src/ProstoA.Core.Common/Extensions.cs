namespace ProstoA.Core;

public static class Extensions
{
    public static TResult Get<TResult>(this Either<TResult, Func<TResult>> value)
        => value
            .Map(x => x, x => x())
            .TryGet(out var result) ? result : throw new InvalidOperationException();

    
    // Для второго аргумента Maybe

    public static TResult GetOrDefault<TResult>(
        this Maybe<TResult> value,
        Either<TResult, Func<TResult>> defaultValue)
        => value.TryGet(out var result) ? result : defaultValue.Get();
    
    public static TResult GetOrDefault<TResult>(
        this Maybe<TResult> value,
        TResult defaultValue)
        => value.GetOrDefault(new Either<TResult, Func<TResult>>(defaultValue));
    
    public static TResult GetOrDefault<TResult>(
        this Maybe<TResult> value,
        Func<TResult> getDefault)
        => value.GetOrDefault(new Either<TResult, Func<TResult>>(getDefault));
    
    
    // Для первого аргумента Either

    public static TResult GetOrDefault<TResult, T2>(
        this Either<TResult, T2> value,
        Either<TResult, Func<TResult>> defaultValue)
        => value.TryGet(out TResult result) ? result : defaultValue.Get();
    
    public static TResult GetOrDefault<TResult, T2>(
        this Either<TResult, T2> value,
        TResult defaultValue)
        => value.GetOrDefault(new Either<TResult, Func<TResult>>(defaultValue));
    
    public static TResult GetOrDefault<TResult, T2>(
        this Either<TResult, T2> value,
        Func<TResult> getDefault)
        => value.GetOrDefault(new Either<TResult, Func<TResult>>(getDefault));
    
    
    // Для второго аргумента Either
    
    public static TResult GetOrDefault<T1, TResult>(
        this Either<T1, TResult> value,
        Either<TResult, Func<TResult>> defaultValue)
        => value.TryGet(out TResult result) ? result : defaultValue.Get();
    
    public static TResult GetOrDefault<T1, TResult>(
        this Either<T1, TResult> value,
        TResult defaultValue)
        => value.GetOrDefault(new Either<TResult, Func<TResult>>(defaultValue));
    
    public static TResult GetOrDefault<T1, TResult>(
        this Either<T1, TResult> value,
        Func<TResult> getDefault)
        => value.GetOrDefault(new Either<TResult, Func<TResult>>(getDefault));
    
    
    // Маперы
    
    public static Maybe<TResult> Map<T, TResult>(
        this Maybe<T> value,
        Func<T, TResult> mapper)
        => value.Map(x => (true, mapper(x)));
    
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