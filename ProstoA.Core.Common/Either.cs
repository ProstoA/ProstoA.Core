namespace ProstoA.Core;

public readonly struct Either<TOne, TTwo> : IAccessor
{
    private readonly TOne? _value1;
    private readonly TTwo? _value2;
    private readonly byte _index;

    public Either()
    {
        _index = 0;
    }
    
    public Either(TOne value)
    {
        _index = 1;
        _value1 = value;
    }
    
    public Either(TTwo value)
    {
        _index = 2;
        _value2 = value;
    }

    public object? Value => _index switch
    {
        0 => default,
        1 => _value1,
        2 => _value2,
        _ => throw new InvalidOperationException()
    };

    public bool TryGetResult(out TOne? value)
    {
        value = _value1;
        return _index == 1;
    }
    
    public bool TryGetResult(out TTwo? value)
    {
        value = _value2;
        return _index == 2;
    }

    public void Do(Action<TOne>? one = default, Action<TTwo>? two = default, Action? empty = default)
    {
        switch (_index)
        {
            case 0:
                empty?.Invoke();
                break;
            case 1:
                one?.Invoke(_value1!);
                break;
            case 2:
                two?.Invoke(_value2!);
                break;
            default:
                throw new InvalidOperationException();
        }
    }
    
    public T Get<T>(Func<T> getDefault) => _index switch
    {
        0 => getDefault(),
        1 => ValueConverter<TOne>.Convert(_value1, getDefault),
        2 => ValueConverter<TTwo>.Convert(_value2, getDefault),
        _ => throw new InvalidOperationException()
    };

    public static implicit operator Either<TOne, TTwo>(TOne result) => new(result);

    public static implicit operator Either<TOne, TTwo?>(TTwo result) => new(result);
}