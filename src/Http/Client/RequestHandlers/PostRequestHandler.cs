namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers;

/// <summary>
/// A handler used to send a post request with content body.
/// </summary>
public class PostRequestHandler<TContent, TResult>
    : ContentRequestHandler<TContent, TResult>
{
    /// <summary>
    /// Creates a new instance of the <see cref="PostRequestHandler{TContent,TResult}"/> class
    /// </summary>
    public PostRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, IHttpHandler? httpHandler = null, string? clientName = null)
        : base(HttpMethod.Post, httpClientFactory, path, httpHandler, clientName)
    {
    }
}
