using System.Collections;
using System.Collections.Specialized;

namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Provides utilities for loading keyed paging data while enforcing key uniqueness.
/// </summary>
/// <typeparam name="TKey">The type of key used to load page data.</typeparam>
/// <typeparam name="TItem">The type of data item.</typeparam>
/// <typeparam name="TId">The unique key type for each item.</typeparam>
public class KeyedPager<TKey, TItem, TId> : IPager<TKey, TItem>
{
    private readonly IDataSource<TKey, TItem> _dataSource;
    private readonly Func<TItem, TId> _idSelector;
    private readonly object _jobsLock = new();
    private readonly object _stateLock = new();

    private record PageKeys(TKey? NextKey, TKey? PrevKey);

    private CancellationTokenSource? _refreshCts;
    private CancellationTokenSource? _appendCts;
    private CancellationTokenSource? _prependCts;

    private PagerState _state = new()
    {
        Refresh = LoadState.NotLoading,
        Append = LoadState.NotLoading,
        Prepend = LoadState.NotLoading
    };

    /// <summary>
    /// A synchronization object for item, key and pending action updates.
    /// </summary>
    protected readonly object _syncRoot = new();

    /// <summary>
    /// The currently loaded items.
    /// </summary>
    protected readonly List<TItem> _items = [];

    /// <summary>
    /// The set of keys currently represented by <see cref="_items"/>.
    /// </summary>
    protected readonly HashSet<TId> _itemIds;

    /// <summary>
    /// The loaded page keys in order.
    /// </summary>
    private readonly List<PageKeys> _keys = [];

    /// <summary>
    /// Queue for mutations that arrive while loading is in progress.
    /// </summary>
    protected readonly List<Action> _pendingActions = [];

    /// <summary>
    /// Creates a keyed pager.
    /// </summary>
    public KeyedPager(
        IDataSource<TKey, TItem> dataSource,
        Func<TItem, TId> idSelector,
        IEqualityComparer<TId>? comparer = null,
        int pageSize = 20,
        bool supportsPrepending = false)
    {
        _dataSource = dataSource;
        _idSelector = idSelector;
        _itemIds = new HashSet<TId>(comparer ?? EqualityComparer<TId>.Default);
        PageSize = pageSize;
        SupportsPrepending = supportsPrepending;
    }

    /// <inheritdoc/>
    public IReadOnlyList<TItem> Items => _items;

    /// <inheritdoc/>
    public int PageSize { get; }

    /// <inheritdoc/>
    public bool SupportsPrepending { get; }

    /// <inheritdoc/>
    public PagerState State
    {
        get
        {
            lock (_stateLock)
            {
                return _state;
            }
        }
        private set
        {
            PagerState? changedState = null;
            lock (_stateLock)
            {
                if (_state == value) return;
                _state = value;
                changedState = _state;
            }

            StateChanged?.Invoke(this, new PagerStateChangedArgs(changedState));
        }
    }

    /// <inheritdoc/>
    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    /// <inheritdoc/>
    public event EventHandler<PagerStateChangedArgs>? StateChanged;

    /// <inheritdoc/>
    public async Task RefreshAsync(CancellationToken cancellationToken = default)
    {
        if (State.Refresh == LoadState.Loading)
        {
            return;
        }

        CancellationTokenSource operationCts;
        lock (_jobsLock)
        {
            CancelAndDispose(ref _refreshCts);
            CancelAndDispose(ref _appendCts);
            CancelAndDispose(ref _prependCts);
            _refreshCts = new CancellationTokenSource();
            operationCts = _refreshCts;
        }

        lock (_syncRoot)
        {
            _keys.Clear();
            _items.Clear();
            _itemIds.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, operationCts.Token);
        var refreshKey = await _dataSource.GetRefreshKeyAsync(linkedCts.Token);
        await LoadCoreAsync(refreshKey, LoadType.Refresh, linkedCts.Token);
    }

    /// <inheritdoc/>
    public void ReachedStart()
    {
        if (!SupportsPrepending)
        {
            return;
        }

        if (State.Prepend == LoadState.Loading || State.Refresh == LoadState.Loading)
        {
            return;
        }

        TKey? firstKey;
        lock (_syncRoot)
        {
            firstKey = _keys.Count == 0 ? default : _keys[0].PrevKey;
        }

        if (firstKey == null)
        {
            return;
        }

        CancellationToken token;
        lock (_jobsLock)
        {
            CancelAndDispose(ref _prependCts);
            _prependCts = new CancellationTokenSource();
            token = _prependCts.Token;
        }

        _ = LoadCoreAsync(firstKey, LoadType.Prepend, token);
    }

    /// <inheritdoc/>
    public void ReachedEnd()
    {
        if (State.Append == LoadState.Loading || State.Refresh == LoadState.Loading)
        {
            return;
        }

        TKey? lastKey;
        lock (_syncRoot)
        {
            lastKey = _keys.Count == 0 ? default : _keys[^1].NextKey;
        }

        if (lastKey == null)
        {
            return;
        }

        CancellationToken token;
        lock (_jobsLock)
        {
            CancelAndDispose(ref _appendCts);
            _appendCts = new CancellationTokenSource();
            token = _appendCts.Token;
        }

        _ = LoadCoreAsync(lastKey, LoadType.Append, token);
    }

    /// <summary>
    /// Tries to find an item by key.
    /// </summary>
    public bool TryFindById(TId id, out TItem item)
    {
        lock (_syncRoot)
        {
            var index = IndexOfIdUnsafe(id);
            if (index < 0)
            {
                item = default!;
                return false;
            }

            item = _items[index];
            return true;
        }
    }

    /// <summary>
    /// Finds an item by key, or <see langword="default"/> when no match exists.
    /// </summary>
    public TItem? FindById(TId id)
    {
        return TryFindById(id, out var item) ? item : default;
    }

    /// <summary>
    /// Finds the index of an item by key.
    /// </summary>
    public int IndexOf(TId id)
    {
        lock (_syncRoot)
        {
            return IndexOfIdUnsafe(id);
        }
    }

    /// <summary>
    /// Finds the index of the first item matching a predicate.
    /// </summary>
    public int IndexOf(Predicate<TItem> predicate)
    {
        lock (_syncRoot)
        {
            return _items.FindIndex(predicate);
        }
    }

    /// <summary>
    /// Gets the first item, or <see langword="default"/> when no items exist.
    /// </summary>
    public TItem? FirstOrDefault()
    {
        lock (_syncRoot)
        {
            return _items.Count == 0 ? default : _items[0];
        }
    }

    /// <summary>
    /// Gets the first item matching a predicate, or <see langword="default"/> when no match exists.
    /// </summary>
    public TItem? FirstOrDefault(Predicate<TItem> predicate)
    {
        lock (_syncRoot)
        {
            var index = _items.FindIndex(predicate);
            return index < 0 ? default : _items[index];
        }
    }

    /// <summary>
    /// Gets the last item, or <see langword="default"/> when no items exist.
    /// </summary>
    public TItem? LastOrDefault()
    {
        lock (_syncRoot)
        {
            return _items.Count == 0 ? default : _items[^1];
        }
    }

    /// <summary>
    /// Gets the last item matching a predicate, or <see langword="default"/> when no match exists.
    /// </summary>
    public TItem? LastOrDefault(Predicate<TItem> predicate)
    {
        lock (_syncRoot)
        {
            var index = _items.FindLastIndex(predicate);
            return index < 0 ? default : _items[index];
        }
    }

    /// <summary>
    /// Gets an item by index, or <see langword="default"/> when the index is out of range.
    /// </summary>
    public TItem? GetOrDefault(int index)
    {
        lock (_syncRoot)
        {
            return index < 0 || index >= _items.Count ? default : _items[index];
        }
    }

    /// <summary>
    /// Gets the key for an item.
    /// </summary>
    protected TId GetId(TItem item)
    {
        return _idSelector(item);
    }

    /// <summary>
    /// Returns true when any load operation is currently in progress.
    /// </summary>
    protected bool IsCurrentlyLoading()
    {
        var state = State;
        return state.Refresh == LoadState.Loading || state.Append == LoadState.Loading || state.Prepend == LoadState.Loading;
    }

    /// <summary>
    /// Runs a mutation immediately when not loading, or queues it to run after the current load finishes.
    /// </summary>
    protected void RunOrQueueMutation(Action mutation)
    {
        lock (_syncRoot)
        {
            if (IsCurrentlyLoading())
            {
                _pendingActions.Add(mutation);
            }
            else
            {
                mutation();
            }
        }
    }

    /// <summary>
    /// Finds the index of an item by key.
    /// </summary>
    protected int IndexOfIdUnsafe(TId id)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_itemIds.Comparer.Equals(GetId(_items[i]), id))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Notifies subscribers that the collection has changed.
    /// </summary>
    protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(this, args);
    }

    private async Task LoadCoreAsync(TKey? key, LoadType type, CancellationToken cancellationToken)
    {
        var request = new LoadRequest<TKey>
        {
            Key = key,
            Type = type,
            PageSize = PageSize
        };

        SetStateForType(type, LoadState.Loading);

        try
        {
            var result = await _dataSource.LoadAsync(request, cancellationToken);
            cancellationToken.ThrowIfCancellationRequested();

            lock (_syncRoot)
            {
                var keys = new PageKeys(result.NextKey, result.PreviousKey);
                var addedItems = ApplyLoadResultUnsafe(request.Type, result.Items, keys);

                if (addedItems.Count > 0)
                {
                    NotifyCollectionChangedEventArgs args = request.Type switch
                    {
                        LoadType.Prepend => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, addedItems, 0),
                        _ => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, addedItems)
                    };
                    OnCollectionChanged(args);
                }

                ProcessPendingMutationsUnsafe();
            }

            SetStateForType(type, LoadState.NotLoading);
        }
        catch (OperationCanceledException)
        {
            SetStateForType(type, LoadState.NotLoading);
        }
        catch
        {
            SetStateForType(type, LoadState.Error);
        }
    }

    private IList ApplyLoadResultUnsafe(LoadType type, IReadOnlyList<TItem> incoming, PageKeys keys)
    {
        List<TItem> added = [];

        switch (type)
        {
            case LoadType.Refresh:
                _items.Clear();
                _itemIds.Clear();
                _keys.Clear();
                foreach (var item in incoming)
                {
                    var id = GetId(item);
                    if (_itemIds.Add(id))
                    {
                        _items.Add(item);
                        added.Add(item);
                    }
                }

                _keys.Add(keys);
                break;

            case LoadType.Append:
                foreach (var item in incoming)
                {
                    var id = GetId(item);
                    if (_itemIds.Add(id))
                    {
                        _items.Add(item);
                        added.Add(item);
                    }
                }

                _keys.Add(keys);
                break;

            case LoadType.Prepend:
                foreach (var item in incoming)
                {
                    var id = GetId(item);
                    if (_itemIds.Add(id))
                    {
                        added.Add(item);
                    }
                }

                if (added.Count > 0)
                {
                    _items.InsertRange(0, added);
                }

                _keys.Insert(0, keys);
                break;

            default:
                throw new InvalidOperationException("Invalid load type");
        }

        return added;
    }

    private void ProcessPendingMutationsUnsafe()
    {
        if (_pendingActions.Count == 0)
        {
            return;
        }

        var actions = _pendingActions.ToArray();
        _pendingActions.Clear();

        foreach (var action in actions)
        {
            action();
        }
    }

    private void SetStateForType(LoadType type, LoadState state)
    {
        var current = State;
        State = type switch
        {
            LoadType.Refresh => current with { Refresh = state },
            LoadType.Prepend => current with { Prepend = state },
            LoadType.Append => current with { Append = state },
            _ => throw new InvalidOperationException("Invalid load type")
        };
    }

    private static void CancelAndDispose(ref CancellationTokenSource? cts)
    {
        if (cts == null)
        {
            return;
        }

        cts.Cancel();
        cts.Dispose();
        cts = null;
    }
}
