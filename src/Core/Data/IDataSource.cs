namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Defines methods for loading paginated data from a data source.
/// </summary>
/// <typeparam name="TKey">The key used to page the data</typeparam>
/// <typeparam name="TItem">The type of data</typeparam>
public interface IDataSource<TKey, TItem>
{
    /// <summary>
    /// Gets the key used to load the data afresh, for example when loading the data for the first time or when the data is invalidated.
    /// </summary>
    Task<TKey?> GetRefreshKeyAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Loads a page of data from the data source.
    /// </summary>
    Task<LoadResult<TKey, TItem>> LoadAsync(LoadRequest<TKey> request, CancellationToken cancellationToken = default);
}
