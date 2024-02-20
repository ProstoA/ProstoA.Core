using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace ProstoA.Core.Operations;

public interface ILocalizationProvider
{
    OperationResult<Localized<T>> Get<T>(string key, params string[] locales);
}

public class DictionaryLocalizationProvider : ILocalizationProvider
{
    private readonly Func<(string[] Keys, string[] Locales), Task<(string Key, (string Locale, object Value)[] Items)[]>> _reader;
    private readonly ConcurrentDictionary<string, Task<(string Key, (string Locale, object Value)[] Items)[]>> _dictionary = new();
    private readonly ConcurrentDictionary<string, Dictionary<string, string[]>> _requested = new();

    public DictionaryLocalizationProvider(
        Func<(string[] Keys, string[] Locales), Task<(string Key, (string Locale, object Value)[])[]>> reader)
    {
        _reader = reader;
    }

    public async OperationResult<Localized<T>> Get<T>(string key, params string[] locales)
    {
        var results = await _dictionary.GetOrAdd(key, _reader((new[] { key }, locales)));
        var variants = results.Select(x => x.Items.ToDictionary(
            xx => xx.Locale,
            xx => xx.Value
        )).FirstOrDefault() ?? new Dictionary<string, object>(0);

        if (!TryGet<T>(variants, locales, out var value))
        {
            OperationResult.Fail();
        }
        
        return value;
    }

    private static bool TryGet<T>(
        IDictionary<string, object> dictionary,
        IEnumerable<string> locales,
        out Localized<T> result)
    {
        foreach (var locale in locales)
        {
            if (dictionary.TryGetValue(locale, out var value))
            {
                result = new Localized<T>((T)value, locale);
                return true;
            }
        }

        result = default;
        return false;
    }
}

public readonly struct Localized<TValue>
{
    public TValue Value { get; }
    public string Locale { get; }

    public Localized(TValue value, string? locale = default)
    {
        Value = value;
        Locale = locale ?? string.Empty; // Thread.CurrentThread.CurrentCulture.Name
    }

    public static implicit operator Localized<TValue>(TValue value) => new(value);
}

public record struct ErrorCode(string Value);

public record struct ErrorMessage(string Value, object[] Args)
{
    public ErrorMessage(string value) : this(value, Array.Empty<object>()) { }
};

public record struct ClientMessage(IDictionary<CultureInfo, string> Values);

public readonly record struct OperationError
{
    public OperationError(string message)
    {
        Message = new ErrorMessage(message);
    }
    
    public OperationError(ErrorMessage message)
    {
        Message = message;
    }
    
    public OperationError(ErrorCode code, ErrorMessage message)
    {
        Code = code;
        Message = message;
    }
    
    public OperationError(ErrorCode code, ErrorMessage message, ClientMessage clientMessage)
    {
        Code = code;
        Message = message;
        ClientMessage = clientMessage;
    }

    ErrorCode Code { get; }
    ErrorMessage Message { get; }
    ClientMessage ClientMessage { get; }

    void Log(ILogger logger)
    {
        logger.LogError(Message.Value, Message.Args);
    }
}