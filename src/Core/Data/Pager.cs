using System.Collections.Specialized;

namespace Ocluse.LiquidSnow.Data;

public class Pager<TKey, TItem>(IDataSource<TKey, TItem> dataSource, int pageSize = 20, bool supportsPrepending = false) : INotifyCollectionChanged
{
    private record PageKeys(TKey? NextKey, TKey? PrevKey);

    protected readonly List<TItem> _items = [];

    private readonly List<PageKeys> _keys = [];

    private PagerState _state = new()
    {
        Refresh = LoadState.NotLoading,
        Append = LoadState.NotLoading,
        Prepend = LoadState.NotLoading
    };

    public IReadOnlyList<TItem> Items => _items;

    public int PageSize => pageSize;

    public bool SupportsPrepending => supportsPrepending;

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

    public event NotifyCollectionChangedEventHandler? CollectionChanged;

    public event EventHandler<PagerStateChangedArgs>? StateChanged;

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

    protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
    {
        CollectionChanged?.Invoke(this, args);
    }
}
