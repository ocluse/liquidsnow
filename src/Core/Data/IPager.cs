using System.Collections.Specialized;

namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Defines a paged data controller that loads items incrementally from an <see cref="IDataSource{TKey, TItem}"/>,
/// tracks loading state for refresh/append/prepend operations, and exposes collection and state change notifications.
/// </summary>
/// <typeparam name="TKey">The type of key used to load the data.</typeparam>
/// <typeparam name="TItem">The type of data item.</typeparam>
public interface IPager<TKey, TItem> : INotifyCollectionChanged
{
    /// <summary>
    /// Gets the current in-memory snapshot of loaded items.
    /// </summary>
    IReadOnlyList<TItem> Items { get; }

    /// <summary>
    /// Gets the maximum number of items requested per load operation.
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Gets whether this pager supports loading items that come before the current first page.
    /// </summary>
    bool SupportsPrepending { get; }

    /// <summary>
    /// Gets the current state of refresh, append, and prepend operations.
    /// </summary>
    PagerState State { get; }

    /// <summary>
    /// Occurs when <see cref="State"/> changes.
    /// </summary>
    event EventHandler<PagerStateChangedArgs>? StateChanged;

    /// <summary>
    /// Clears currently loaded items and reloads data from the source using the refresh key.
    /// </summary>
    Task RefreshAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Notifies the pager that the data accessor has reached the start and should load more prepending data when supported.
    /// </summary>
    void ReachedStart();

    /// <summary>
    /// Notifies the pager that the data accessor has reached the end and should load more appending data when available.
    /// </summary>
    void ReachedEnd();
}
