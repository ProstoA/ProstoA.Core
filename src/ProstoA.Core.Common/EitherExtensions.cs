namespace ProstoA.Core;

public static class EitherExtensions
{
    public static Maybe<TResult> MapT1<T1, T2, TResult>(this Either<T1, T2> value, Func<T1, TResult> mapper)
        => value.Map(mapper1: mapper, mapper2: null);
    
    public static Maybe<TResult> MapT2<T1, T2, TResult>(this Either<T1, T2> value, Func<T2, TResult> mapper)
        => value.Map(mapper1: null, mapper2: mapper);
    
    public static T1 GetOrDefault<T1, T2>(this Either<T1, T2> value, T1 defaultValue)
        => value.TryGet(out T1 result) ? result : defaultValue;
    
    public static T2 GetOrDefault<T1, T2>(this Either<T1, T2> value, T2 defaultValue)
        => value.TryGet(out T2 result) ? result : defaultValue;
    
    public static void Do<T1, T2>(
        this Either<T1, T2> value,
        Action<T1>? t1 = default,
        Action<T2>? t2 = default,
        Action? empty = default)
    {

        var action = value.Map<Action>(
            x => () => { t1?.Invoke(x); },
            x => () => { t2?.Invoke(x); }
        ).TryGet(out var result) ? result : empty;
        
        action?.Invoke();
    }
}