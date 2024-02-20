namespace ProstoA.Core;

public interface IValueAccessor
{
    (bool, TResult) Get<TResult>();

    bool TryGet<TResult>(out TResult result)
    { 
        (var ok, result) = Get<TResult>();
        return ok;
    }
}

public interface IValueContainer<T, out TContainer> : IValueAccessor
{
    public static abstract TContainer None { get; }
    
    bool TryGet(out T value);
}

public interface IValueContainer<T1, T2, out TContainer> : IValueContainer<T1, TContainer>
{
    bool TryGet(out T2 value);
}

public interface IAccessor<in TContainer>
{
    static abstract TResult Get<TResult>(TContainer container, Either<TResult, Func<TResult>> defaultValue);
}