using System.Collections.Specialized;

namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// A keyed pager that supports mutating items while preserving key uniqueness.
/// </summary>
/// <typeparam name="TKey">The type of key used to load page data.</typeparam>
/// <typeparam name="TItem">The type of data item.</typeparam>
/// <typeparam name="TId">The unique key type for each item.</typeparam>
public class MutableKeyedPager<TKey, TItem, TId> : KeyedPager<TKey, TItem, TId>
{
    /// <summary>
    /// Creates a mutable keyed pager.
    /// </summary>
    public MutableKeyedPager(
        IDataSource<TKey, TItem> dataSource,
        Func<TItem, TId> idSelector,
        IEqualityComparer<TId>? comparer = null,
        int pageSize = 20,
        bool supportsPrepending = false)
        : base(dataSource, idSelector, comparer, pageSize, supportsPrepending)
    {
    }

    /// <summary>
    /// Adds a new item, or updates an existing item with the same key.
    /// </summary>
    public void AddOrUpdate(TItem item, int? atIndex = null, ConflictStrategy strategy = ConflictStrategy.Replace)
    {
        RunOrQueueMutation(() =>
        {
            var itemId = GetId(item);
            var existingIndex = IndexOfIdUnsafe(itemId);

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
                        throw new InvalidOperationException($"Item with id '{itemId}' already exists in the list.");

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
                _itemIds.Add(itemId);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, index));
            }
        });
    }

    /// <summary>
    /// Forces an item to be inserted at a given index by removing any existing item with the same key.
    /// </summary>
    public void ForceAdd(TItem item, int atIndex = 0)
    {
        RunOrQueueMutation(() =>
        {
            var itemId = GetId(item);
            var existingIndex = IndexOfIdUnsafe(itemId);

            if (existingIndex != -1)
            {
                var existing = _items[existingIndex];
                _items.RemoveAt(existingIndex);
                OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, existing, existingIndex));
            }
            else
            {
                _itemIds.Add(itemId);
            }

            int target = Math.Clamp(atIndex, 0, _items.Count);
            _items.Insert(target, item);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, item, target));
        });
    }

    /// <summary>
    /// Updates an item only when an item with the same key already exists.
    /// </summary>
    public void UpdateIfExists(TItem item)
    {
        RunOrQueueMutation(() =>
        {
            var itemId = GetId(item);
            var existingIndex = IndexOfIdUnsafe(itemId);
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
    public void RemoveById(TId id)
    {
        RunOrQueueMutation(() =>
        {
            var existingIndex = IndexOfIdUnsafe(id);
            if (existingIndex == -1)
            {
                return;
            }

            var existing = _items[existingIndex];
            _items.RemoveAt(existingIndex);
            _itemIds.Remove(id);
            OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, existing, existingIndex));
        });
    }
}
