﻿namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers;

/// <summary>
/// A request handler that sends a request with content.
/// </summary>

public class ContentRequestHandler<TContent, TResult>(
    HttpMethod httpMethod, 
    ISnowHttpClientFactory httpClientFactory, 
    string path, 
    IHttpHandler? httpHandler = null, 
    string? clientName = null) 
    : RequestHandler<TResult>(httpClientFactory, path, httpHandler, clientName)
{

    /// <summary>
    /// The HTTP method used to send the request.
    /// </summary>
    public HttpMethod HttpMethod { get; } = httpMethod;

    /// <summary>
    /// Gets the path to send the request to.
    /// </summary>
    /// <remarks>
    /// The path returned by this method will be transformed by <see cref="GetTransformedUrlPath(TContent)"/> before sending the request.
    /// </remarks>
    protected virtual string GetUrlPath(TContent content)
    {
        return Path;
    }

    /// <summary>
    /// Gets the transformed path to send the request to.
    /// </summary>
    /// <remarks>
    /// This is the actual path that will be sent to the server.
    /// </remarks>
    protected virtual string GetTransformedUrlPath(TContent content)
    {
        string path = GetUrlPath(content);

        return TransformUrlPath(path);
    }

    /// <summary>
    /// Sends a request with the given content, returning the result.
    /// </summary>
    public async Task<TResult> ExecuteAsync(TContent content, CancellationToken cancellationToken = default)
    {
        string path = GetTransformedUrlPath(content);
        HttpContent httpContent = await GetContent(content, cancellationToken);

        using HttpRequestMessage request = new(HttpMethod, path);

        request.Content = httpContent;

        return await SendAsync(request, cancellationToken);
    }
}
