namespace ProstoA.Core;

public static class EitherExtensions
{
    public static Maybe<TResult> MapT1<T1, T2, TResult>(this Either<T1, T2> value, Func<T1, TResult> mapper)
        => value.Map(x => (true, mapper(x)), _ => (false, default!));
    
    public static Maybe<TResult> MapT2<T1, T2, TResult>(this Either<T1, T2> value, Func<T2, TResult> mapper)
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