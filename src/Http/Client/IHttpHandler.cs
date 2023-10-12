using System.Text.Json;

namespace Ocluse.LiquidSnow.Http.Client
{
    /// <summary>
    /// A handler used to transform HTTP requests and responses.
    /// </summary>
    public interface IHttpHandler
    {
        
    }

    /// <summary>
    /// A handler that provides the default name of the client to be used where none is specified.
    /// </summary>
    public interface IClientNameProvider : IHttpHandler
    {
        /// <summary>
        /// The client name used when creating HTTP clients.
        /// </summary>
        string ClientName { get; }
    }

    /// <summary>
    /// A handler that provides other handlers
    /// </summary>
    public interface IHttpHandlerProvider : IHttpHandler
    {
        /// <summary>
        /// Gets the handler of the specified type, or null if none is found.
        /// </summary>
        T? GetHandler<T>() where T : IHttpHandler;
    }

    /// <summary>
    /// A handler that provides options for serializing and deserializing JSON.
    /// </summary>
    public interface IJsonOptionsProvider
    {
        /// <summary>
        /// The options used to serialize and deserialize JSON.
        /// </summary>
        JsonSerializerOptions JsonSerializerOptions { get; }
    }

    /// <summary>
    /// A handler invoked before an HTTP request is sent.
    /// </summary>
    public interface IHttpRequestHandler : IHttpHandler
    {
        /// <summary>
        /// Handles the request before it is sent, for example to add headers.
        /// </summary>
        Task HandleRequestBeforeSend(HttpRequestMessage request, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// A handler invoked after an HTTP response is received.
    /// </summary>
    public interface IHttpResponseHandler : IHttpHandler
    {
        /// <summary>
        /// Handles the response after it is received, for example to check for errors.
        /// </summary>
        Task HandleResponseAfterReceive(HttpResponseMessage response, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// A handler used to transform the HTTP request URL, for example to add a version prefix.
    /// </summary>
    public interface IHttpUrlTransformer : IHttpHandler
    {
        /// <summary>
        /// Transforms the URL.
        /// </summary>
        string Transform(string url);
    }

    /// <summary>
    /// A handler used to transform a value into a query string parameter.
    /// </summary>
    public interface IHttpQueryTransformer : IHttpHandler
    {
        /// <summary>
        /// Transforms the value into a query string parameter.
        /// </summary>
        string Transform<T>(T value);
    }

    /// <summary>
    /// A handler used for HTTP request/response bodies.
    /// </summary>
    public interface IHttpContentHandler : IHttpHandler
    {
        /// <summary>
        /// Transforms the value into an HTTP content, for example by serializing it to JSON.
        /// </summary>
        Task<HttpContent> GetContent<T>(T value, CancellationToken cancellationToken = default);

        /// <summary>
        /// Transforms the body of the HTTP response into a value, for example by deserializing it from JSON.
        /// </summary>
        Task<TResult> GetResult<TResult>(HttpResponseMessage response, CancellationToken cancellationToken = default);
    }
}
