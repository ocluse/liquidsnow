using System.Collections.Specialized;

namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Loads and manages paged data from an <see cref="IDataSource{TCursor, TItem}"/>.
/// Tracks refresh/append/prepend load states and emits collection and state notifications as items are loaded.
/// </summary>
/// <typeparam name="TCursor">The type of cursor used to load the data.</typeparam>
/// <typeparam name="TItem">The type of data.</typeparam>
/// <param name="dataSource">The source used to load the data.</param>
/// <param name="pageSize">The maximum number of items to load on each operation.</param>
/// <param name="supportsPrepending">Indicates whether the pager can prepend data, i.e. load data from the page before the initial page (i.e. page 0).</param>
public class Pager<TCursor, TItem>(IDataSource<TCursor, TItem> dataSource, int pageSize = 20, bool supportsPrepending = false) : IPager<TCursor, TItem>
{
    private record PageCursors(TCursor? NextCursor, TCursor? PreviousCursor);

    /// <summary>
    /// The items loaded by the pager.
    /// </summary>
    protected readonly List<TItem> _items = [];

    private readonly List<PageCursors> _cursors = [];

    private PagerState _state = new()
    {
        Refresh = LoadState.NotLoading,
        Append = LoadState.NotLoading,
        Prepend = LoadState.NotLoading
    };

    /// <summary>
    /// Gets the current in-memory snapshot of loaded items.
    /// </summary>
    public IReadOnlyList<TItem> Items => _items;

    /// <summary>
    /// Gets the maximum number of items requested per load operation.
    /// </summary>
    public int PageSize => pageSize;

    /// <summary>
    /// Gets whether this pager supports loading items that come before the current first page.
    /// </summary>
    public bool SupportsPrepending => supportsPrepending;

    /// <summary>
    /// Gets the current state of refresh, append, and prepend operations.
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
    /// Clears currently loaded items and reloads data from the source using the refresh cursor.
    /// </summary>
    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        //do not refresh if we are already refreshing:
        if (State.Refresh == LoadState.Loading)
        {
            return;
        }

        _cursors.Clear();
        ClearItems();
        var refreshCursor = await dataSource.GetRefreshCursorAsync(cancellationToken);
        await LoadCoreAsync(refreshCursor, LoadType.Refresh, cancellationToken);
    }

    /// <summary>
    /// Notifies the pager that the data accessor has reached the start and should load more prepending data when supported.
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
        if (_cursors.Count == 0)
        {
            return;
        }
        var firstCursor = _cursors[0].PreviousCursor;
        if (firstCursor == null)
        {
            return;
        }

        _ = LoadCoreAsync(firstCursor, LoadType.Prepend, CancellationToken.None);
    }

    /// <summary>
    /// Notifies the pager that the data accessor has reached the end and should load more appending data when available.
    /// </summary>
    public void ReachedEnd()
    {
        //do not append if we are already appending:
        if (State.Append == LoadState.Loading)
        {
            return;
        }

        //reached the end of the scroll list, do we need to load more?
        if (_cursors.Count == 0)
        {
            return;
        }
        var lastCursor = _cursors[^1].NextCursor;
        if (lastCursor == null)
        {
            return;
        }

        _ = LoadCoreAsync(lastCursor, LoadType.Append, CancellationToken.None);
    }
    private async Task LoadCoreAsync(TCursor? cursor, LoadType type, CancellationToken cancellationToken = default)
    {
        LoadRequest<TCursor> request = new()
        {
            Cursor = cursor,
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

            PageCursors cursors = new(result.NextCursor, result.PreviousCursor);

            if (result.Items.Count > 0)
            {
                if (request.Type == LoadType.Prepend)
                {
                    _items.InsertRange(0, result.Items);
                    _cursors.Insert(0, cursors);
                }
                else if (request.Type == LoadType.Append)
                {
                    _items.AddRange(result.Items);
                    _cursors.Add(cursors);
                }
                else
                {
                    _items.AddRange(result.Items);
                    _cursors.Add(cursors);
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
