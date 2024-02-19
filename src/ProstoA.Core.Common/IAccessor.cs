namespace ProstoA.Core;

public interface IAccessor
{
    TResult Get<TResult>(Func<TResult> getDefault);
}

public static class AccessorExtensions
{
    public static TResult GetOrDefault<TResult>(this IAccessor accessor, TResult defaultValue) => accessor.Get(() => defaultValue);
}