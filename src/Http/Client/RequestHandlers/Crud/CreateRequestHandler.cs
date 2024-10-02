namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers.Crud;

/// <summary>
/// A handler user to create a resource
/// </summary>
public class CreateRequestHandler<TCreate, TResult> : PostRequestHandler<TCreate, TResult>
{
    /// <summary>
    /// Creates a new instance of the <see cref="CreateRequestHandler{TCreate,TResult}"/> class
    /// </summary>
    public CreateRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, IHttpHandler? httpHandler = null, string ? clientName = null) 
        : base(httpClientFactory, path, httpHandler, clientName)
    {
    }
}
