namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers;

/// <summary>
/// A handler used to send get requests to a resource.
/// </summary>
public class GetRequestHandler<TResult>(
    ISnowHttpClientFactory httpClientFactory, 
    string path, IHttpHandler? httpHandler = null,
    string? clientName = null) 
    : RequestHandler<TResult>(httpClientFactory, path, httpHandler, clientName)
{

    /// <summary>
    /// Gets the url path for the request.
    /// </summary>
    /// <remarks>
    /// The path returned by this method will be transformed by the <see cref="GetTransformedUrlPath"/> method before making the request.
    /// </remarks>
    protected virtual string GetUrlPath()
    {
        return Path;
    }

    /// <summary>
    /// The method used to make the final transformation to the url path before making the request
    /// </summary>
    protected virtual string GetTransformedUrlPath()
    {
        string path = GetUrlPath();

        return TransformUrlPath(path);
    }

    /// <summary>
    /// Sends a request with the given id, returning the result.
    /// </summary>
    public async Task<TResult> ExecuteAsync(CancellationToken cancellationToken = default)
    {
        string path = GetTransformedUrlPath();

        using HttpRequestMessage request = new(HttpMethod.Get, path);
        return await SendAsync(request, cancellationToken);
    }
}

