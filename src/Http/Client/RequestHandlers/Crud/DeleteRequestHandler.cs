using System.Reactive;

namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers.Crud
{
    /// <summary>
    /// A handler user to delete a resource
    /// </summary>
    public class DeleteRequestHandler : DeleteByIdRequestHandler<Unit>
    {
        /// <summary>
        /// Creates a new instance of the <see cref="DeleteRequestHandler"/> class
        /// </summary>
        public DeleteRequestHandler(ISnowHttpClientFactory httpClientFactory, string path, IHttpHandler? httpHandler = null, string? clientName = null) 
            : base(httpClientFactory, path, httpHandler, clientName)
        {
        }
    }
}
