namespace ProstoA.Core;

public interface IValueContainer<T, out TContainer>
    where TContainer : IValueContainer<T, TContainer>
{
    public static abstract TContainer None { get; }
    
    bool TryGet(out T value);
}

public interface IValueContainer<T1, T2, out TContainer> : IValueContainer<T1, TContainer>
    where TContainer : IValueContainer<T1, T2, TContainer>
{
    bool TryGet(out T2 value);
}