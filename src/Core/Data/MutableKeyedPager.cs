using System.Collections.Specialized;

namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// A keyed pager that supports mutating items while preserving key uniqueness.
/// </summary>
/// <typeparam name="TCursor">The type of cursor used to load page data.</typeparam>
/// <typeparam name="TItem">The type of data item.</typeparam>
/// <typeparam name="TKey">The unique key type for each item.</typeparam>
public class MutableKeyedPager<TCursor, TItem, TKey> : KeyedPager<TCursor, TItem, TKey>
    where TKey : notnull
{
    /// <summary>
    /// Creates a mutable keyed pager.
    /// </summary>
    /// <param name="dataSource">The source used to load paged data.</param>
    /// <param name="keySelector">A function that extracts the unique key from each item.</param>
    /// <param name="keyComparer">
    /// An optional key comparer for uniqueness checks. When <see langword="null"/>,
    /// <see cref="EqualityComparer{T}.Default"/> is used.
    /// </param>
    /// <param name="pageSize">The maximum number of items to request per load operation.</param>
    /// <param name="supportsPrepending">
    /// Indicates whether prepending is supported when <see cref="KeyedPager{TCursor, TItem, TKey}.ReachedStart"/> is called.
    /// </param>
    /// <param name="loadConflictStrategy">
    /// Specifies how duplicate keys encountered during load operations are handled.
    /// Defaults to <see cref="ConflictStrategy.Ignore"/>.
    /// </param>
    public MutableKeyedPager(
        IDataSource<TCursor, TItem> dataSource,
        Func<TItem, TKey> keySelector,
        IEqualityComparer<TKey>? keyComparer = null,
        int pageSize = 20,
        bool supportsPrepending = false,
        ConflictStrategy loadConflictStrategy = ConflictStrategy.Ignore)
        : base(dataSource, keySelector, keyComparer, pageSize, supportsPrepending, loadConflictStrategy)
    {
    }

    /// <summary>
    /// Adds a new item, or updates an existing item with the same key. The index of the the existing item is preserved when updating, and the new item is inserted at the specified index when adding.
    /// </summary>
    /// <param name="item">The item to add or update.</param>
    /// <param name="atIndex">Optional insertion index when the item does not already exist.</param>
    /// <param name="strategy">How to resolve key conflicts when an item with the same key already exists.</param>
    public void AddOrUpdate(TItem item, int? atIndex = null, ConflictStrategy strategy = ConflictStrategy.Replace)
    {
        RunOrQueueMutation(() =>
        {
            var itemKey = GetItemKey(item);
            var existingIndex = IndexOfKeyUnsafe(itemKey);

            if (existingIndex != -1)
            {
                switch (strategy)
                {
                    case ConflictStrategy.Replace:
                        var oldItem = _items[existingIndex];
                        _items[existingIndex] = item;
                        OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, existingIndex));
                        break;

                    case ConflictStrategy.Ignore:
                        break;

                    case ConflictStrategy.Error:
                        throw new InvalidOperationException($"Item with key '{itemKey}' already exists in the list.");

                    default:
                        throw new InvalidOperationException("Invalid conflict strategy.");
                }
            }
            else
            {
                int index = atIndex.HasValue
                    ? Math.Clamp(atIndex.Value, 0, _items.Count)
                    : _items.Count;

                _items.Insert(index, item);
                _itemKeys.Add(itemKey);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
        });
    }

    /// <summary>
    /// Forces an item to be inserted at a given index by removing any existing item with the same key.
    /// </summary>
    /// <param name="item">The item to insert.</param>
    /// <param name="atIndex">The preferred insertion index. If not provided, the item is added at the end of the list.</param>
    public void ForceAdd(TItem item, int? atIndex = null)
    {
        RunOrQueueMutation(() =>
        {
            var itemKey = GetItemKey(item);
            var existingIndex = IndexOfKeyUnsafe(itemKey);

            if (existingIndex != -1)
            {
                var existing = _items[existingIndex];
                _items.RemoveAt(existingIndex);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, existing, existingIndex));
            }
            else
            {
                _itemKeys.Add(itemKey);
            }

            int index = atIndex.HasValue
                   ? Math.Clamp(atIndex.Value, 0, _items.Count)
                   : _items.Count;

            _items.Insert(index, item);

            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
        });
    }

    /// <summary>
    /// Updates an item only when an item with the same key already exists.
    /// </summary>
    /// <param name="item">The replacement item.</param>
    public void UpdateIfExists(TItem item)
    {
        RunOrQueueMutation(() =>
        {
            var itemKey = GetItemKey(item);
            var existingIndex = IndexOfKeyUnsafe(itemKey);
            if (existingIndex == -1)
            {
                return;
            }

            var oldItem = _items[existingIndex];
            _items[existingIndex] = item;
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, item, oldItem, existingIndex));
        });
    }

    /// <summary>
    /// Removes an item by key.
    /// </summary>
    /// <param name="key">The key of the item to remove.</param>
    public void RemoveByKey(TKey key)
    {
        RunOrQueueMutation(() =>
        {
            var existingIndex = IndexOfKeyUnsafe(key);
            if (existingIndex == -1)
            {
                return;
            }

            var existing = _items[existingIndex];
            _items.RemoveAt(existingIndex);
            _itemKeys.Remove(key);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, existing, existingIndex));
        });
    }
}
