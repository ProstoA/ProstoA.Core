namespace ProstoA.Core.Security.AccessControl;

public class RateLimit
{
    // todo: добавить информации об окне лимита, если требуется
    
    /// <summary>
    /// The maximum number of requests that the consumer is permitted to make per hour.
    /// </summary>
    public int Limit { get; set; }

    /// <summary>
    /// The number of requests remaining in the current rate limit window.
    /// </summary>
    public int Remaining { get; set; }
    
    /// <summary>
    /// The time at which the current rate limit window resets in UTC epoch seconds.
    /// </summary>
    public int /*Timstamp*/ Reset { get; set; }
    
    /*HTTP/1.1 429
    Date: Tue, 20 Aug 2013 14:50:41 GMT
    Status: 429
    X-RateLimit-Limit: 60
    X-RateLimit-Remaining: 0
    X-RateLimit-Reset: 1377013266
    {
        "message": "Too many requests",
        "code": 429
    }*/
    
    /*HTTP/1.1 429
    Content-Type: application/json; charset=utf-8
    Connection: close
    {
        "message": "Too many login attempts",
        "code": 429
    }*/
}