using System.Reflection;

namespace ProstoA.Core;

public static class ValueConverter<TValue>
{
    public static TValue GetDefault()
    {
        return default!;
    }
    
    public static (bool, TResult) Convert<TResult>(TValue? source)
    {
        // todo: оптимизировать конвертер если TValue совпадает с TResult
        
        if (WrapValue(source, out TResult? v)) return (true, v!);
        if (WrapAccessor(source, out TResult? r)) return (true, r!);
        if (WrapArray(source, out TResult? a)) return (true, a!);
        
        return default;
    }

    private static bool WrapValue<TResult>(TValue? value, out TResult? result)
    {
        var canExec = value is not null && typeof(TValue).IsAssignableTo(typeof(TResult));
        
        result = canExec ? (TResult)(object)value! : default;
        return canExec;
    }

    private static T Get<T, TContainer>(TContainer container, Either<T, Func<T>> defaultValue)
    {
        return (T) container.GetType()
            .GetMethod(nameof(IAccessor<T>.Get), BindingFlags.Public | BindingFlags.Static)
            .MakeGenericMethod(typeof(T))
            .Invoke(null, new object[] { container, defaultValue });
    }
    
    private static bool WrapAccessor<TResult>(TValue? value, out TResult? result)
    {
        var container = value as IAccessor<TValue>;
        var canExec = container is not null;
        
        result = canExec ? Get<TResult, TValue>(value!, new Either<TResult, Func<TResult>>(() =>
        {
            canExec = false;
            return default!;
        })) : default;

        return canExec;
    }
    
    private static bool WrapArray<TResult>(TValue? value, out TResult? result)
    {
        var resultType = typeof(TResult);
        var canExec = value is not null && resultType.IsArray && resultType.GetElementType() == typeof(TValue);
        
        result = canExec ? (TResult)(object)new[] { value } : default;
        return canExec;
    }
}