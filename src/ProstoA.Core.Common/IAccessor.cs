namespace ProstoA.Core;

public interface IAccessor<in TContainer>
{
    static abstract TResult Get<TResult>(TContainer container, Either<TResult, Func<TResult>> defaultValue);
}

public static class AccessorExtensions
{
    public static TResult GetOrDefault<TResult, TContainer>(this TContainer container, TResult orDefault)
        where TContainer : IAccessor<TContainer>
        => TContainer.Get(container, new Either<TResult, Func<TResult>>(orDefault));
    
    public static TResult GetOrDefault<TResult, TContainer>(this TContainer container, Func<TResult> getDefault)
        where TContainer : IAccessor<TContainer>
        => TContainer.Get(container, new Either<TResult, Func<TResult>>(getDefault));
}