namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers;

/// <summary>
/// A handler that sends http request to a server and returns a result
/// </summary>
public class UriRequestHandler<TResult> : RequestHandler<TResult>
{
    /// <summary>
    /// Creates a new instance of the <see cref="UriRequestHandler{TResult}"/> class
    /// </summary>
    public UriRequestHandler(HttpMethod httpMethod, ISnowHttpClientFactory httpClientFactory, string path, IHttpHandler? httpHandler = null, string? clientName = null)
        : base(httpClientFactory, path, httpHandler, clientName)
    {
        HttpMethod = httpMethod;
    }

    /// <summary>
    /// The http method to use when sending the request
    /// </summary>
    public HttpMethod HttpMethod { get; }

    /// <summary>
    /// Gets the url path to send the request to by combining the <see cref="Path"/> and the <paramref name="uri"/>.
    /// </summary>
    protected virtual string GetUrlPath(string? uri)
    {
        return Path + uri;
    }
    /// <summary>
    /// The final transformation applied to the url path before sending the request.
    /// </summary>
    protected virtual string GetTransformedUrlPath(string? uri)
    {
        string path = GetUrlPath(uri);

        return TransformUrlPath(path);
    }

    /// <summary>
    /// Sends the request with the given <paramref name="uri"/> and returns the result
    /// </summary>
    public async Task<TResult> ExecuteAsync(string? uri, CancellationToken cancellationToken = default)
    {
        string path = GetTransformedUrlPath(uri);
        using HttpRequestMessage request = new(HttpMethod, path);
        return await SendAsync(request, cancellationToken);
    }
}
