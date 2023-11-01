using System.Globalization;
using Microsoft.Extensions.Logging;

namespace ProstoA.Core.Operations;

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