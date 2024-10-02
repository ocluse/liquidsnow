namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers.Crud;

/// <summary>
/// A handler user to get a list of resources
/// </summary>
public class ListRequestHandler<TQuery, TResult> : GetByQueryRequestHandler<TQuery, QueryResult<TResult>>
{
    /// <summary>
    /// Creates a new instance of the <see cref="ListRequestHandler{TQuery,TResult}"/> class
    /// </summary>
    public ListRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, IHttpHandler? httpHandler = null, string? clientName = null) 
        : base(httpClientFactory, path, httpHandler, clientName)
    {
    }
}
