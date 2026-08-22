namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Defines methods for loading paginated data from a data source.
/// </summary>
/// <typeparam name="TCursor">The cursor used to page the data.</typeparam>
/// <typeparam name="TItem">The type of data</typeparam>
public interface IDataSource<TCursor, TItem>
{
    /// <summary>
    /// Gets the cursor used to load the data afresh, for example when loading the data for the first time or when the data is invalidated.
    /// </summary>
    Task<TCursor?> GetRefreshCursorAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads a page of data from the data source.
    /// </summary>
    Task<LoadResult<TCursor, TItem>> LoadAsync(LoadRequest<TCursor> request, CancellationToken cancellationToken = default);
}
