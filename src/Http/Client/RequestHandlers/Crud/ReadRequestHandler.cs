namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers.Crud
{
    /// <summary>
    /// A handler user to get a resource by id
    /// </summary>
    /// <typeparam name="TResult"></typeparam>
    public class ReadRequestHandler<TResult> : GetByIdRequestHandler<TResult>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="ReadRequestHandler{TResult}"/> class
        /// </summary>
        public ReadRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, string? clientName = null, IHttpHandler? httpHandler = null) 
            : base(httpClientFactory, path, clientName, httpHandler)
        {
        }
    }
}
