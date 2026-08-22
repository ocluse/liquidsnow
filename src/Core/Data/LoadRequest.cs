namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Represents a request to load data from a data source.
/// </summary>
/// <typeparam name="TCursor">The type of cursor used to page the data.</typeparam>
public record LoadRequest<TCursor>
{
    /// <summary>
    /// The cursor identifying the page of data to load.
    /// </summary>
    public required TCursor? Cursor { get; init; }

    /// <summary>
    /// The type of load operation to perform.
    /// </summary>
    public required LoadType Type { get; init; }

    /// <summary>
    /// The maximum number of items to load in the request.
    /// </summary>
    public required int PageSize { get; init; }
}
