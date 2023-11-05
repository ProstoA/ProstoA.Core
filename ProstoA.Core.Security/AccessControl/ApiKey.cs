namespace ProstoA.Core.Security.AccessControl;

/// <summary>
/// API keys are secrets used by Server to prove their identity.
/// What can be accessed each API key is restricted by scopes instead of permissions.
/// </summary>
/// <remarks>
/// It is a best practice to grant only the scopes you need to meet your project's goals to an API key.
/// API keys should be treated as a secret.
/// Never share the API key and keep API keys out of client applications.
/// </remarks>
public class ApiKey
{
    public string Name { get; set; }
    
    public DateTimeOffset? ExpirationDate { get; set; } // null == Never
    
    /*
    Scopes
    ======
    Choose which permission scopes to grant your application.
    It is best practice to allow only the permissions you need to meet your project goals.
    
    https://appwrite.io/docs/advanced/platform/api-keys
    */
}