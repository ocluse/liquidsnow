using System.Collections.Specialized;

namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Provides utilities for loading paging data, keeping track of the current state of the data, and notifying when the data changes.
/// </summary>
/// <typeparam name="TKey">The type of key used to load the data.</typeparam>
/// <typeparam name="TItem">The type of data.</typeparam>
/// <param name="dataSource">The source used to load the data.</param>
/// <param name="pageSize">The maximum number of items to load on each operation.</param>
/// <param name="supportsPrepending">Indicates whether the pager can prepend data, i.e. load data from the page before the initial page (i.e. page 0).</param>
public class Pager<TKey, TItem>(IDataSource<TKey, TItem> dataSource, int pageSize = 20, bool supportsPrepending = false) : INotifyCollectionChanged
{
    private record PageKeys(TKey? NextKey, TKey? PrevKey);

    /// <summary>
    /// The items loaded by the pager.
    /// </summary>
    protected readonly List<TItem> _items = [];

    private readonly List<PageKeys> _keys = [];

    private PagerState _state = new()
    {
        Refresh = LoadState.NotLoading,
        Append = LoadState.NotLoading,
        Prepend = LoadState.NotLoading
    };

    /// <summary>
    /// Returns the list of items that have currently been loaded.
    /// </summary>
    public IReadOnlyList<TItem> Items => _items;

    /// <summary>
    /// Returns the maximum number of items to load on each operation.
    /// </summary>
    public int PageSize => pageSize;

    /// <summary>
    /// Returns a value indicating whether the pager can prepend data.
    /// </summary>
    public bool SupportsPrepending => supportsPrepending;

    /// <summary>
    /// Returns the current state of the pager.
    /// </summary>
    public PagerState State
    {
        get => _state;
        set
        {
            if (_state != value)
            {
                _state = value;
                StateChanged?.Invoke(this, new PagerStateChangedArgs(_state));
            }
        }
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <summary>
    /// Occurs when the state of the pager changes.
    /// </summary>
    public event EventHandler<PagerStateChangedArgs>? StateChanged;

    /// <summary>
    /// Refreshes the data in the pager. This will clear all items and load the data from the data source again.
    /// </summary>
    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        //do not refresh if we are already refreshing:
        if (State.Refresh == LoadState.Loading)
        {
            return;
        }

        _keys.Clear();
        ClearItems();
        var refreshKey = await dataSource.GetRefreshKeyAsync(cancellationToken);
        await LoadCoreAsync(refreshKey, LoadType.Refresh, cancellationToken);
    }

    /// <summary>
    /// Notifies the pager that it has reached the start of the data. This will load more data if the pager supports prepending.
    /// </summary>
    public void ReachedStart()
    {
        if (!SupportsPrepending) return;

        //do not prepend if we are already prepending:
        if (State.Prepend == LoadState.Loading)
        {
            return;
        }

        //reached the start of the scroll list, do we need to load more?
        if (_keys.Count == 0)
        {
            return;
        }
        var firstKey = _keys[0].PrevKey;
        if (firstKey == null)
        {
            return;
        }

        _ = LoadCoreAsync(firstKey, LoadType.Prepend, CancellationToken.None);
    }

    /// <summary>
    /// Notifies the pager that it has reached the end of the data. This will load more data if there are more items to load.
    /// </summary>
    public void ReachedEnd()
    {
        //do not append if we are already appending:
        if (State.Append == LoadState.Loading)
        {
            return;
        }

        //reached the end of the scroll list, do we need to load more?
        if (_keys.Count == 0)
        {
            return;
        }
        var lastKey = _keys[^1].NextKey;
        if (lastKey == null)
        {
            return;
        }

        _ = LoadCoreAsync(lastKey, LoadType.Append, CancellationToken.None);
    }
    private async Task LoadCoreAsync(TKey? key, LoadType type, CancellationToken cancellationToken = default)
    {
        LoadRequest<TKey> request = new()
        {
            Key = key,
            Type = type,
            PageSize = PageSize
        };

        State = request.Type switch
        {
            LoadType.Refresh => State with { Refresh = LoadState.Loading },
            LoadType.Prepend => State with { Prepend = LoadState.Loading },
            LoadType.Append => State with { Append = LoadState.Loading },
            _ => throw new InvalidOperationException("Invalid load direction")
        };

        try
        {
            var result = await dataSource.LoadAsync(request, cancellationToken);

            PageKeys keys = new(result.NextKey, result.PreviousKey);

            if (result.Items.Count > 0)
            {
                if (request.Type == LoadType.Prepend)
                {
                    _items.InsertRange(0, result.Items);
                    _keys.Insert(0, keys);
                }
                else if (request.Type == LoadType.Append)
                {
                    _items.AddRange(result.Items);
                    _keys.Add(keys);
                }
                else
                {
                    _items.AddRange(result.Items);
                    _keys.Add(keys);
                }

                NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Add, result.Items);
                CollectionChanged?.Invoke(this, args);
            }

            State = request.Type switch
            {
                LoadType.Refresh => State with { Refresh = LoadState.NotLoading },
                LoadType.Prepend => State with { Prepend = LoadState.NotLoading },
                LoadType.Append => State with { Append = LoadState.NotLoading },
                _ => throw new InvalidOperationException("Invalid load direction")
            };
        }
        catch
        {
            State = request.Type switch
            {
                LoadType.Refresh => State with { Refresh = LoadState.Error },
                LoadType.Prepend => State with { Prepend = LoadState.Error },
                LoadType.Append => State with { Append = LoadState.Error },
                _ => throw new InvalidOperationException("Invalid load direction")
            };
        }
    }

    private void ClearItems()
    {
        _items.Clear();
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Reset);
        CollectionChanged?.Invoke(this, args);
    }

    /// <summary>
    /// Notifies the subscribers of the collection changed event.
    /// </summary>
    protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(this, args);
    }
}
