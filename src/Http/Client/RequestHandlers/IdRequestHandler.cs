﻿namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers;

/// <summary>
/// A request handler used to send a request with an id based path
/// </summary>d
public class IdRequestHandler<TKey, TResult>(
    HttpMethod httpMethod,
    ISnowHttpClientFactory httpClientFactory,
    string path,
    IHttpHandler? httpHandler = null,
    string? clientName = null) : RequestHandler<TResult>(httpClientFactory, path, httpHandler, clientName)
{

    /// <summary>
    /// The http method to use for the request
    /// </summary>
    public HttpMethod HttpMethod { get; } = httpMethod;

    /// <summary>
    /// Gets the url path for the request
    /// </summary>
    /// <remarks>
    /// The path returned by this method will be transformed by the <see cref="GetTransformedUrlPath"/> method before making the request.
    /// </remarks>
    protected virtual string GetUrlPath(TKey id)
    {
        return $"{Path}/{GetPathSegmentFromId(id)}";
    }

    /// <summary>
    /// The method used to make the final transformation to the url path before making the request
    /// </summary>
    protected virtual string GetTransformedUrlPath(TKey id)
    {
        string path = GetUrlPath(id);

        return TransformUrlPath(path);
    }

    /// <summary>
    /// Sends a request with the given id, returning the result.
    /// </summary>
    public async Task<TResult> ExecuteAsync(TKey id, CancellationToken cancellationToken = default)
    {
        string path = GetTransformedUrlPath(id);

        using HttpRequestMessage request = new(HttpMethod, path);
        return await SendAsync(request, cancellationToken);
    }
}
