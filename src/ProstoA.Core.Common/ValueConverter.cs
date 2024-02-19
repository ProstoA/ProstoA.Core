using System.Reflection;

namespace ProstoA.Core;

public static class ValueConverter<TValue>
{
    public static T Convert<T>(TValue? value, Func<T> getDefault)
    {
        if (WrapValue(value, out T? v)) return v!;
        if (WrapAccessor(value, out T? r)) return r!;
        if (WrapArray(value, out T? a)) return a!;

        return getDefault();
    }

    private static bool WrapValue<T>(TValue? value, out T? result)
    {
        var canExec = value is not null && typeof(TValue).IsAssignableTo(typeof(T));
        
        result = canExec ? (T)(object)value! : default;
        return canExec;
    }

    private static T Get<T, TContainer>(TContainer container, Func<T> orDefault)
    {
        return (T) container.GetType()
            .GetMethod(nameof(IAccessor<T>.Get), BindingFlags.Public | BindingFlags.Static)
            .MakeGenericMethod(typeof(T))
            .Invoke(null, new object[] { container, orDefault });
    }
    
    private static bool WrapAccessor<T>(TValue? value, out T? result)
    {
        var container = value as IAccessor<TValue>;
        var canExec = container is not null;
        
        result = canExec ? Get<T, TValue>(value!, () =>
        {
            canExec = false;
            return default!;
        }) : default;

        return canExec;
    }
    
    private static bool WrapArray<T>(TValue? value, out T? result)
    {
        var resultType = typeof(T);
        var canExec = value is not null && resultType.IsArray && resultType.GetElementType() == typeof(TValue);
        
        result = canExec ? (T)(object)new[] { value } : default;
        return canExec;
    }
}