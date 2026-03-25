using System.Collections.Specialized;

namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Defines the common contract for pagers.
/// </summary>
/// <typeparam name="TKey">The type of key used to load the data.</typeparam>
/// <typeparam name="TItem">The type of data item.</typeparam>
public interface IPager<TKey, TItem> : INotifyCollectionChanged
{
    /// <summary>
    /// Returns the list of items that have currently been loaded.
    /// </summary>
    IReadOnlyList<TItem> Items { get; }

    /// <summary>
    /// Returns the maximum number of items to load on each operation.
    /// </summary>
    int PageSize { get; }

    /// <summary>
    /// Returns a value indicating whether the pager can prepend data.
    /// </summary>
    bool SupportsPrepending { get; }

    /// <summary>
    /// Returns the current state of the pager.
    /// </summary>
    PagerState State { get; }

    /// <summary>
    /// Occurs when the state of the pager changes.
    /// </summary>
    event EventHandler<PagerStateChangedArgs>? StateChanged;

    /// <summary>
    /// Refreshes the data in the pager.
    /// </summary>
    Task RefreshAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Notifies the pager that it has reached the start of the data.
    /// </summary>
    void ReachedStart();

    /// <summary>
    /// Notifies the pager that it has reached the end of the data.
    /// </summary>
    void ReachedEnd();
}
