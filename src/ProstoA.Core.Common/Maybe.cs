namespace ProstoA.Core;

public readonly struct Maybe<T> : IAccessor
{
    public static readonly Maybe<T> None = new();

    private readonly T _value;
    private readonly bool _hasValue;

    public Maybe(T value)
    {
        _value = value;
        _hasValue = true;
    }

    public bool TryGet(out T value)
    {
        value = _value;
        return _hasValue;
    }
    
    public static implicit operator Maybe<T>(T value) => new(value);

    public TResult Get<TResult>(Func<TResult> getDefault) => TryGet(out var result)
        ? ValueConverter<T>.Convert(result, getDefault)
        : getDefault();
}