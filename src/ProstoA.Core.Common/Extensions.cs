namespace ProstoA.Core;

public static class Extensions
{
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