using System.Collections.Concurrent;
using System.Globalization;
using Microsoft.Extensions.Logging;

namespace ProstoA.Core.Operations;

public interface ILocalizationProvider
{
    Task<Localized<T>> Get<T>(string key, Func<Localized<T>> getDefault, params string[] locales);
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

    public async Task<Localized<T>> Get<T>(string key, Func<Localized<T>> getDefault, params string[] locales)
    {
        var results = await _dictionary.GetOrAdd(key, _reader((new[] { key }, locales)));
        var variants = results.Select(x => x.Items.ToDictionary(
            xx => xx.Locale,
            xx => xx.Value
        )).FirstOrDefault() ?? new Dictionary<string, object>(0);
        
        return TryGet<T>(variants, locales, out var value) ? value! : getDefault();
    }

    private static bool TryGet<T>(
        IDictionary<string, object> dictionary,
        IEnumerable<string> locales,
        out Localized<T>? result)
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

public class Localized<TValue>
{
    public Localized(TValue value, string? locale = default)
    {
        locale ??= string.Empty; // Thread.CurrentThread.CurrentCulture.Name
    }

    public static implicit operator Localized<TValue>(TValue value) => new(value);
}

public record struct ErrorCode(string Value);

public record struct ErrorMessage(string Value, object[] Args);

public record struct ClientMessage(IDictionary<CultureInfo, string> Values);

public record struct OperationError
{
    public OperationError(ErrorCode code, ErrorMessage message)
    {
        Code = code;
        Message = message;
    }

    ErrorCode Code { get; }
    ErrorMessage Message { get; }

    ClientMessage? ClientMessage { get; set; }

    void Log(ILogger logger)
    {
        logger.LogError(Message.Value, Message.Args);
    }
}