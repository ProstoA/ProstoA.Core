namespace ProstoA.Core;

public interface IAccessor
{
    T Get<T>(Func<T> getDefault);
}

public static class AccessorExtensions
{
    public static T GetOrDefault<T>(this IAccessor accessor, T defaultValue) => accessor.Get(() => defaultValue);
}