using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Extensions;

/// <summary>
/// Extensions for <see cref="HttpClient"/> to simplify various HTTP requests.
/// </summary>
public static class HttpExtensions
{
    #region Post as Form
    ///<inheritdoc cref="PostAsFormAsync{T}(HttpClient, Uri?, T, JsonSerializerOptions?, CancellationToken)"/>
    public static async Task<HttpResponseMessage> PostAsFormAsync<T>(this HttpClient httpClient, Uri? requestUri, T content, CancellationToken cancellationToken = default)
    {
        return await PostAsFormAsync(httpClient, requestUri, content, null, cancellationToken).ConfigureAwait(false);
    }

    ///<inheritdoc cref="PostAsFormAsync{T}(HttpClient, Uri?, T, JsonSerializerOptions?, CancellationToken)"/>
    public static async Task<HttpResponseMessage> PostAsFormAsync<T>(this HttpClient httpClient, string? requestUri, T content, CancellationToken cancellationToken = default)
    {
        return await PostAsFormAsync(httpClient, requestUri?.ToUri(), content, null, cancellationToken).ConfigureAwait(false);
    }

    ///<inheritdoc cref="PostAsFormAsync{T}(HttpClient, Uri?, T, JsonSerializerOptions?, CancellationToken)"/>
    public static async Task<HttpResponseMessage> PostAsFormAsync<T>(this HttpClient httpClient, string? requestUri, T content, JsonSerializerOptions? options, CancellationToken cancellationToken = default)
    {
        return await PostAsFormAsync(httpClient, requestUri?.ToUri(), content, options, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a POST request with form data structured from the provided content object.
    /// </summary>
    public static async Task<HttpResponseMessage> PostAsFormAsync<T>(this HttpClient httpClient, Uri? requestUri, T content, JsonSerializerOptions? options, CancellationToken cancellationToken = default)
    {
        string jsonContent = JsonSerializer.Serialize(content, options);
        var tuples = JsonSerializer.Deserialize<IDictionary<string, object>>(jsonContent, options)!
            .Where(x => x.Value != null)
            .Select(x => new Tuple<string, object>(x.Key, x.Value))
            .ToList();

        var collections = tuples.Where(x => x.Item2 is JsonElement y && y.ValueKind == JsonValueKind.Array).ToList();

        //remove all collections:
        foreach (var collection in collections)
        {
            tuples.Remove(collection);
        }

        //add back the collections as a list of new tuples:
        foreach (var collection in collections)
        {
            var enumerable = (JsonElement)collection.Item2;
            foreach (var item in enumerable.EnumerateArray())
            {
                tuples.Add(new Tuple<string, object>(collection.Item1, item));
            }
        }

        Dictionary<string, string> formData = [];
        foreach (var tuple in tuples)
        {
            if (tuple.Item2 is JsonElement element && element.ValueKind == JsonValueKind.String)
            {
                formData.Add(tuple.Item1, element.GetString()!);
            }
            else if (tuple.Item2 is string str)
            {
                formData.Add(tuple.Item1, str);
            }
            else
            {
                formData.Add(tuple.Item1, tuple.Item2.ToString()!);
            }
        }

        using var contentForm = new FormUrlEncodedContent(formData);
        return await httpClient.PostAsync(requestUri, contentForm, cancellationToken).ConfigureAwait(false);
    }

    #endregion

    #region Put as Form
    ///<inheritdoc cref="PutAsFormAsync{T}(HttpClient, Uri?, T, JsonSerializerOptions?, CancellationToken)"/>
    public static async Task<HttpResponseMessage> PutAsFormAsync<T>(this HttpClient httpClient, Uri? requestUri, T content, CancellationToken cancellationToken = default)
    {
        return await PutAsFormAsync(httpClient, requestUri, content, null, cancellationToken).ConfigureAwait(false);
    }

    ///<inheritdoc cref="PutAsFormAsync{T}(HttpClient, Uri?, T, JsonSerializerOptions?, CancellationToken)"/>
    public static async Task<HttpResponseMessage> PutAsFormAsync<T>(this HttpClient httpClient, string? requestUri, T content, CancellationToken cancellationToken = default)
    {
        return await PutAsFormAsync(httpClient, requestUri?.ToUri(), content, null, cancellationToken).ConfigureAwait(false);
    }

    ///<inheritdoc cref="PutAsFormAsync{T}(HttpClient, Uri?, T, JsonSerializerOptions?, CancellationToken)"/>
    public static async Task<HttpResponseMessage> PutAsFormAsync<T>(this HttpClient httpClient, string? requestUri, T content, JsonSerializerOptions? options, CancellationToken cancellationToken = default)
    {
        return await PutAsFormAsync(httpClient, requestUri?.ToUri(), content, options, cancellationToken).ConfigureAwait(false);
    }

    /// <summary>
    /// Sends a PUT request with form data structured from the provided content object.
    /// </summary>
    public static async Task<HttpResponseMessage> PutAsFormAsync<T>(this HttpClient httpClient, Uri? requestUri, T content, JsonSerializerOptions? options, CancellationToken cancellationToken = default)
    {
        string jsonContent = JsonSerializer.Serialize(content, options);
        var tuples = JsonSerializer.Deserialize<IDictionary<string, object>>(jsonContent, options)!
            .Where(x => x.Value != null)
            .Select(x => new Tuple<string, object>(x.Key, x.Value))
            .ToList();

        var collections = tuples.Where(x => x.Item2 is JsonElement y && y.ValueKind == JsonValueKind.Array).ToList();

        //remove all collections:
        foreach (var collection in collections)
        {
            tuples.Remove(collection);
        }

        //add back the collections as a list of new tuples:
        foreach (var collection in collections)
        {
            var enumerable = (JsonElement)collection.Item2;
            foreach (var item in enumerable.EnumerateArray())
            {
                tuples.Add(new Tuple<string, object>(collection.Item1, item));
            }
        }

        Dictionary<string, string> formData = [];
        foreach (var tuple in tuples)
        {
            if (tuple.Item2 is JsonElement element && element.ValueKind == JsonValueKind.String)
            {
                formData.Add(tuple.Item1, element.GetString()!);
            }
            else if (tuple.Item2 is string str)
            {
                formData.Add(tuple.Item1, str);
            }
            else
            {
                formData.Add(tuple.Item1, tuple.Item2.ToString()!);
            }
        }

        using var contentForm = new FormUrlEncodedContent(formData);
        return await httpClient.PutAsync(requestUri, contentForm, cancellationToken).ConfigureAwait(false);
    }

    #endregion
}
