using System.Collections;
using System.Collections.Specialized;

namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// Loads and manages keyed paged data from an <see cref="IDataSource{TCursor, TItem}"/> while ensuring
/// that only one item per key exists in <see cref="Items"/>.
/// Duplicate keys from refresh, append, and prepend loads are ignored.
/// </summary>
/// <typeparam name="TCursor">The type of cursor used to load page data.</typeparam>
/// <typeparam name="TItem">The type of data item.</typeparam>
/// <typeparam name="TKey">The unique key type for each item.</typeparam>
public class KeyedPager<TCursor, TItem, TKey> : IPager<TCursor, TItem>, IItemKeyProvider<TItem>
    where TKey : notnull
{
    private readonly IDataSource<TCursor, TItem> _dataSource;
    private readonly Func<TItem, TKey> _keySelector;
    private readonly ConflictStrategy _loadConflictStrategy;
    private readonly object _jobsLock = new();
    private readonly object _stateLock = new();

    private record PageCursors(TCursor? NextCursor, TCursor? PreviousCursor);

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
    protected readonly HashSet<TKey> _itemKeys;

    /// <summary>
    /// The loaded page keys in order.
    /// </summary>
    private readonly List<PageCursors> _cursors = [];

    /// <summary>
    /// Queue for mutations that arrive while loading is in progress.
    /// Queued mutations run immediately after the current load operation finishes.
    /// </summary>
    protected readonly List<Action> _pendingActions = [];

    /// <summary>
    /// Creates a keyed pager.
    /// </summary>
    /// <param name="dataSource">The source used to load paged data.</param>
    /// <param name="keySelector">A function that extracts the unique key from each item.</param>
    /// <param name="keyComparer">
    /// An optional key comparer for uniqueness checks. When <see langword="null"/>,
    /// <see cref="EqualityComparer{T}.Default"/> is used.
    /// </param>
    /// <param name="pageSize">The maximum number of items to request per load operation.</param>
    /// <param name="supportsPrepending">
    /// Indicates whether this pager should respond to <see cref="ReachedStart"/> by attempting a prepend load.
    /// </param>
    /// <param name="loadConflictStrategy">
    /// Specifies how duplicate keys encountered during load operations (refresh, append, prepend) are handled.
    /// Defaults to <see cref="ConflictStrategy.Ignore"/>, which keeps the existing item and drops the duplicate.
    /// </param>
    public KeyedPager(
        IDataSource<TCursor, TItem> dataSource,
        Func<TItem, TKey> keySelector,
        IEqualityComparer<TKey>? keyComparer = null,
        int pageSize = 20,
        bool supportsPrepending = false,
        ConflictStrategy loadConflictStrategy = ConflictStrategy.Ignore)
    {
        _dataSource = dataSource;
        _keySelector = keySelector;
        _loadConflictStrategy = loadConflictStrategy;
        _itemKeys = new HashSet<TKey>(keyComparer ?? EqualityComparer<TKey>.Default);
        PageSize = pageSize;
        SupportsPrepending = supportsPrepending;
    }

    /// <summary>
    /// Gets the current in-memory snapshot of loaded items.
    /// </summary>
    public IReadOnlyList<TItem> Items => _items;

    /// <summary>
    /// Gets the maximum number of items requested per load operation.
    /// </summary>
    public int PageSize { get; }

    /// <summary>
    /// Gets whether this pager supports loading items that come before the current first page.
    /// </summary>
    public bool SupportsPrepending { get; }

    /// <summary>
    /// Gets the current state of refresh, append, and prepend operations.
    /// </summary>
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

    /// <summary>
    /// Clears currently loaded items and reloads data from the source using the refresh cursor.
    /// Any in-flight append or prepend operations are cancelled before the refresh begins.
    /// </summary>
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
            _cursors.Clear();
            _items.Clear();
            _itemKeys.Clear();
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }

        using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, operationCts.Token);
        var refreshCursor = await _dataSource.GetRefreshCursorAsync(linkedCts.Token);
        await LoadCoreAsync(refreshCursor, LoadType.Refresh, linkedCts.Token);
    }

    /// <summary>
    /// Notifies the pager that the data accessor has reached the start and should load more prepending data when supported.
    /// If prepending is unsupported, already loading, or there is no previous cursor, this call does nothing.
    /// </summary>
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

        TCursor? firstCursor;
        lock (_syncRoot)
        {
            firstCursor = _cursors.Count == 0 ? default : _cursors[0].PreviousCursor;
        }

        if (firstCursor == null)
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

        _ = LoadCoreAsync(firstCursor, LoadType.Prepend, token);
    }

    /// <summary>
    /// Notifies the pager that the data accessor has reached the end and should load more appending data when available.
    /// If already loading or there is no next cursor, this call does nothing.
    /// </summary>
    public void ReachedEnd()
    {
        if (State.Append == LoadState.Loading || State.Refresh == LoadState.Loading)
        {
            return;
        }

        TCursor? lastCursor;
        lock (_syncRoot)
        {
            lastCursor = _cursors.Count == 0 ? default : _cursors[^1].NextCursor;
        }

        if (lastCursor == null)
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

        _ = LoadCoreAsync(lastCursor, LoadType.Append, token);
    }

    /// <summary>
    /// Tries to find an item by key.
    /// </summary>
    public bool TryFindByKey(TKey key, out TItem item)
    {
        lock (_syncRoot)
        {
            var index = IndexOfKeyUnsafe(key);
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
    public TItem? FindByKey(TKey key)
    {
        return TryFindByKey(key, out var item) ? item : default;
    }

    /// <summary>
    /// Finds the index of an item by key.
    /// </summary>
    public int IndexOfKey(TKey key)
    {
        lock (_syncRoot)
        {
            return IndexOfKeyUnsafe(key);
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
    public TKey GetItemKey(TItem item)
    {
        return _keySelector(item);
    }

    object IItemKeyProvider<TItem>.GetItemKey(TItem item)
        => GetItemKey(item);

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
    protected int IndexOfKeyUnsafe(TKey key)
    {
        for (int i = 0; i < _items.Count; i++)
        {
            if (_itemKeys.Comparer.Equals(GetItemKey(_items[i]), key))
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

    private async Task LoadCoreAsync(TCursor? cursor, LoadType type, CancellationToken cancellationToken)
    {
        var request = new LoadRequest<TCursor>
        {
            Cursor = cursor,
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
                var cursors = new PageCursors(result.NextCursor, result.PreviousCursor);
                var applyResult = ApplyLoadResultUnsafe(request.Type, result.Items, cursors);

                if (applyResult.HadReplacements)
                {
                    OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                }
                else if (applyResult.AddedItems.Count > 0)
                {
                    NotifyCollectionChangedEventArgs args = request.Type switch
                    {
                        LoadType.Prepend => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, applyResult.AddedItems, 0),
                        _ => new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, applyResult.AddedItems)
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

    private readonly record struct ApplyResult(IList AddedItems, bool HadReplacements);

    private ApplyResult ApplyLoadResultUnsafe(LoadType type, IReadOnlyList<TItem> incoming, PageCursors cursors)
    {
        List<TItem> added = [];
        bool hadReplacements = false;

        switch (type)
        {
            case LoadType.Refresh:
                _items.Clear();
                _itemKeys.Clear();
                _cursors.Clear();
                foreach (var item in incoming)
                {
                    if (ApplyIncomingItemUnsafe(item, LoadType.Append, added))
                    {
                        hadReplacements = true;
                    }
                }

                _cursors.Add(cursors);
                break;

            case LoadType.Append:
                foreach (var item in incoming)
                {
                    if (ApplyIncomingItemUnsafe(item, LoadType.Append, added))
                    {
                        hadReplacements = true;
                    }
                }

                _cursors.Add(cursors);
                break;

            case LoadType.Prepend:
                foreach (var item in incoming)
                {
                    if (ApplyIncomingItemUnsafe(item, LoadType.Prepend, added))
                    {
                        hadReplacements = true;
                    }
                }

                if (added.Count > 0)
                {
                    _items.InsertRange(0, added);
                }

                _cursors.Insert(0, cursors);
                break;

            default:
                throw new InvalidOperationException("Invalid load type");
        }

        return new ApplyResult(added, hadReplacements);
    }

    private bool ApplyIncomingItemUnsafe(TItem item, LoadType type, List<TItem> added)
    {
        var key = GetItemKey(item);
        var existingIndex = IndexOfKeyUnsafe(key);

        if (existingIndex >= 0)
        {
            switch (_loadConflictStrategy)
            {
                case ConflictStrategy.Ignore:
                    return false;

                case ConflictStrategy.Replace:
                    _items[existingIndex] = item;
                    return true;

                case ConflictStrategy.Error:
                    throw new InvalidOperationException($"Duplicate item key '{key}' encountered during {type} load.");

                default:
                    throw new InvalidOperationException("Invalid conflict strategy.");
            }
        }

        _itemKeys.Add(key);

        if (type == LoadType.Prepend)
        {
            added.Add(item);
        }
        else
        {
            _items.Add(item);
            added.Add(item);
        }

        return false;
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
