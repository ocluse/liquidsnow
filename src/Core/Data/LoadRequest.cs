namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Represents a request to load data from a data source.
/// </summary>
/// <typeparam name="TKey">The type of key used to page the data.</typeparam>
public record LoadRequest<TKey>
{
    /// <summary>
    /// The key used to identify the page of data to load.
    /// </summary>
    public required TKey? Key { get; init; }

    /// <summary>
    /// The type of load operation to perform.
    /// </summary>
    public required LoadType Type { get; init; }

    /// <summary>
    /// The maximum number of items to load in the request.
    /// </summary>
    public required int PageSize { get; init; }
}
