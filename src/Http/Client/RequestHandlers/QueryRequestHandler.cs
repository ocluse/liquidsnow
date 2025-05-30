﻿namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers;

/// <summary>
/// A handler that sends http request to a server by query string parameters.
/// </summary>
public class QueryRequestHandler<TQuery, TResult>(
    HttpMethod httpMethod, 
    ISnowHttpClientFactory httpClientFactory, 
    string path,
    IHttpHandler? httpHandler = null,
    string? clientName = null) 
    : RequestHandler<TResult>(httpClientFactory, path, httpHandler, clientName)
{

    /// <summary>
    /// The http method to use when sending the request
    /// </summary>
    public HttpMethod HttpMethod { get; } = httpMethod;

    /// <summary>
    /// Converts the query object into a path with a query string.
    /// </summary>
    protected virtual string GetUrlPath(TQuery? query)
    {
        if (query == null)
        {
            return Path;
        }
        return $"{Path}?{GetQueryString(query)}";
    }
    /// <summary>
    /// The final transformation applied to the url path before sending the request.
    /// </summary>
    protected virtual string GetTransformedUrlPath(TQuery? query)
    {
        string path = GetUrlPath(query);

        return TransformUrlPath(path);
    }

    /// <summary>
    /// Sends the request with the given query string parameters, returning the result.
    /// </summary>
    public async Task<TResult> ExecuteAsync(TQuery? query, CancellationToken cancellationToken = default)
    {
        string path = GetTransformedUrlPath(query);
        using HttpRequestMessage request = new(HttpMethod, path);
        return await SendAsync(request, cancellationToken);
    }
}
