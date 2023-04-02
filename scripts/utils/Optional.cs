namespace Game.Utils;

public struct Optional<T>
{
    public static Optional<T> Empty()
    {
        return new Optional<T>(false);
    }

    public bool HasValue { get; private set; }
    public T Value
    {
        get => _value;
        set
        {
            HasValue = true;
            _value = value;
        }
    }

    private T _value;

    public Optional(bool hasValue)
    {
        HasValue = hasValue;
        _value = default;
    }

    public Optional(T value)
    {
        HasValue = true;
        _value = value;
    }

    public void Clear()
    {
        HasValue = false;
        _value = default;
    }
}