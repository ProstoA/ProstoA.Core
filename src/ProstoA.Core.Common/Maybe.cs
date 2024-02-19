namespace ProstoA.Core;

public readonly record struct Maybe<T> : IAccessor<Maybe<T>>
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
    
    public static Maybe<T> Wrap<TData>(TData value, Func<TData, (bool, T)> mapper)
    {
        var (ok, result) = mapper(value);
        return ok ? new Maybe<T>(result) : None;
    }
    
    public Maybe<TResult> Map<TResult>(Func<T, (bool, TResult)> mapper) => _hasValue switch
    {
        false => Maybe<TResult>.None,
        true => Maybe<TResult>.Wrap(_value, mapper)
    };
    
    public static implicit operator Maybe<T>(T value) => new(value);

    public static TResult Get<TResult>(Maybe<T> container, Func<TResult> getDefault)
    {
        return container
            .Map(ValueConverter<T>.Convert<TResult>)
            .TryGet(out var result) ? result : getDefault();
    }
}