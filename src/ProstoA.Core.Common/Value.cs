namespace ProstoA.Core;

public readonly struct Value<T>
{
    public static readonly Value<T> None = new();

    private readonly T _value;
    private readonly bool _hasValue;

    public Value(T value)
    {
        _value = value;
        _hasValue = true;
    }

    public bool TryGet(out T value)
    {
        value = _value;
        return _hasValue;
    }
    
    public static implicit operator Value<T>(T value) => new(value);
}