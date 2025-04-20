namespace Ocluse.LiquidSnow.Data;

public interface IDataSource<TKey, TItem>
{
    Task<TKey?> GetRefreshKeyAsync(CancellationToken cancellationToken = default);

    Task<LoadResult<TKey, TItem>> LoadAsync(LoadRequest<TKey> request, CancellationToken cancellationToken = default);
}
