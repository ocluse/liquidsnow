using Ocluse.LiquidSnow.Entities;
using System.Net.Http.Json;
using System.Reactive;
using System.Text;
using System.Text.Json;
using System.Web;

namespace Ocluse.LiquidSnow.Http.Client.RequestHandlers;

internal static class JsonOptionsCache
{
    static JsonSerializerOptions? _options;

    public static JsonSerializerOptions Options
    {
        get
        {
            _options ??= new JsonSerializerOptions(JsonSerializerDefaults.Web);

            return _options;
        }
    }
}

/// <summary>
/// An HTTP request handler that sends a request and returns a result.
/// </summary>
public class RequestHandler<TResult>
{
    /// <summary>
    /// Creates a http request handler.
    /// </summary>
    /// <param name="httpClientFactory">The factory that will be used to create HTTP clients</param>
    /// <param name="path">The path that will be used to send the requests</param>
    /// <param name="clientName">The client name that will be created to send the requests. If none is specified, the default one will be used</param>
    /// <param name="httpHandler">The handler that will be used to transform requests and responses</param>
    public RequestHandler(
        ISnowHttpClientFactory httpClientFactory,
        string path,
        string? clientName = null,
        IHttpHandler? httpHandler = null)
    {
        HttpClientFactory = httpClientFactory;
        Path = path;
        HttpHandler = httpHandler;

        //Set the client name:
        if (string.IsNullOrEmpty(clientName))
        {
            var clientNameProvider = HttpHandler.As<IClientNameProvider>();

            if (clientNameProvider != null)
            {
                ClientName = clientNameProvider.ClientName;
            }
            else
            {
                throw new InvalidOperationException("No client name specified and no default client name set.");
            }
        }
        else
        {
            ClientName = clientName;
        }
    }

    /// <summary>
    /// The factory used to create HTTP clients for sending requests.
    /// </summary>
    public ISnowHttpClientFactory HttpClientFactory { get; }

    /// <summary>
    /// The relative path used to send the request.
    /// </summary>
    public string Path { get; }

    /// <summary>
    /// The name of HTTP clients to use when sending the request.
    /// </summary>
    public string ClientName { get; }

    /// <summary>
    /// The handler used to transform the requests and responses.
    /// </summary>
    public IHttpHandler? HttpHandler { get; }

    /// <summary>
    /// The <see cref="JsonSerializerOptions"/> used to serialize and deserialize the request and response.
    /// </summary>
    protected virtual JsonSerializerOptions JsonSerializerOptions
    {
        get
        {
            if (HttpHandler is IJsonOptionsProvider jsonOptionsProvider)
            {
                return jsonOptionsProvider.JsonSerializerOptions;
            }
            else
            {
                return JsonOptionsCache.Options;
            }
        }
    }

    
    /// <summary>
    /// Transforms a path by using the <see cref="IHttpUrlTransformer"/> if one is specified.
    /// </summary>
    protected virtual string TransformUrlPath(string urlPath)
    {
        var urlTransformer = HttpHandler.As<IHttpUrlTransformer>();

        if (urlTransformer != null)
        {
            return urlTransformer.Transform(urlPath);
        }

        return urlPath;
    }

    /// <summary>
    /// Sends a request and returns a result, invoking the <see cref="IHttpRequestHandler"/> and <see cref="IHttpResponseHandler"/> if specified.
    /// </summary>
    public async Task<TResult> SendAsync(HttpRequestMessage requestMessage, CancellationToken cancellationToken)
    {
        var httpRequestHandler = HttpHandler.As<IHttpRequestHandler>();

        if (httpRequestHandler != null)
        {
            await httpRequestHandler.HandleRequestBeforeSend(requestMessage, cancellationToken);
        }

        HttpClient httpClient = await GetClient(cancellationToken);
        HttpResponseMessage response = await httpClient.SendAsync(requestMessage, cancellationToken);

        var httpResponseHandler = HttpHandler.As<IHttpResponseHandler>();

        if (httpResponseHandler != null)
        {
            await httpResponseHandler.HandleResponseAfterReceive(response, cancellationToken);
        }

        return await GetResult(response, cancellationToken);
    }

    /// <summary>
    /// Returns a <see cref="HttpClient"/> with the given or default name.
    /// </summary>
    protected virtual async Task<HttpClient> GetClient(CancellationToken cancellationToken)
    {
        return await HttpClientFactory.CreateClient(ClientName, cancellationToken);
    }

    /// <summary>
    /// Returns the result of the given <see cref="HttpResponseMessage"/>, invoking the <see cref="IHttpContentHandler"/> if specified.
    /// </summary>
    /// <exception cref="ResponseContentNullException"></exception>
    public virtual async Task<TResult> GetResult(HttpResponseMessage message, CancellationToken cancellationToken = default)
    {
        if (HttpHandler is IHttpContentHandler contentHandler)
        {
            return await contentHandler.GetResult<TResult>(message, cancellationToken);
        }
        else if (typeof(TResult) == typeof(Unit))
        {
            return (TResult)(object)Unit.Default;
        }
        else if (typeof(TResult) == typeof(HttpResponseMessage))
        {
            return (TResult)(object)message;
        }
        else
        {
            return await message.Content.ReadFromJsonAsync<TResult>(JsonSerializerOptions, cancellationToken)
                ?? throw new ResponseContentNullException();
        }
    }

    /// <summary>
    /// Returns the equivalent http content of the given value, invoking the <see cref="IHttpContentHandler"/> if specified.
    /// </summary>
    public virtual async Task<HttpContent> GetContent<T>(T value, CancellationToken cancellationToken = default)
    {
        if (HttpHandler is IHttpContentHandler contentHandler)
        {
            return await contentHandler.GetContent(value, cancellationToken);
        }
        else if(value is HttpContent content)
        {
            return content;
        }
        else
        {
            string json = JsonSerializer.Serialize(value, JsonSerializerOptions);

            return new StringContent(json, Encoding.UTF8, "application/json");
        }
    }

    /// <summary>
    /// Transforms the given value into a query string, invoking the <see cref="IHttpQueryTransformer"/> if specified.
    /// </summary>
    protected string GetQueryString<T>(T value)
    {
        if (HttpHandler is IHttpQueryTransformer transformer)
        {
            return transformer.Transform(value);
        }
        else
        {
            var step1 = JsonSerializer.Serialize(value, JsonSerializerOptions);

            var step2 = JsonSerializer.Deserialize<IDictionary<string, object>>(step1, JsonSerializerOptions)!
                .Where(x => x.Value != null)
                .Select(x => new Tuple<string, object>(x.Key, x.Value))
                .ToList();

            var collections = step2.Where(x => x.Item2 is JsonElement y && y.ValueKind == JsonValueKind.Array).ToList();

            //remove all collections:
            foreach (var collection in collections)
            {
                step2.Remove(collection);
            }

            //add back the collections as a list of new tuples:
            foreach (var collection in collections)
            {
                var enumerable = (JsonElement)collection.Item2;
                foreach (var item in enumerable.EnumerateArray())
                {
                    step2.Add(new Tuple<string, object>(collection.Item1, item));
                }
            }

            var step3 = step2.Where(x => x.Item2 != null)
                .Select(x => HttpUtility.UrlEncode(x.Item1) + "=" + HttpUtility.UrlEncode(x.Item2.ToString()));

            return string.Join("&", step3);
        }
    }
}
