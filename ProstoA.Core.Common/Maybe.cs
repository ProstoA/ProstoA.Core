namespace ProstoA.Core;

public readonly struct Maybe<TValue> : IAccessor
{
    private readonly TValue? _value1;
    private readonly byte _index;
    
    public Maybe()
    {
        _index = 0;
    }
    
    public Maybe(TValue value)
    {
        _index = 1;
        _value1 = value;
    }
    
    public object? Value => _index switch
    {
        0 => default,
        1 => _value1,
        _ => throw new InvalidOperationException()
    };
    
    public bool TryGetResult(out TValue? value)
    {
        value = _value1;
        return _index == 1;
    }
    
    public T Get<T>(Func<T> getDefault) => _index switch
    {
        0 => getDefault(),
        1 => ValueConverter<TValue>.Convert(_value1, getDefault),
        _ => throw new InvalidOperationException()
    };

    public static implicit operator Maybe<TValue>(TValue result) => new(result);
}