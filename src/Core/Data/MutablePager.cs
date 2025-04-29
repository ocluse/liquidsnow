using System.Collections.Specialized;

namespace Ocluse.LiquidSnow.Data;

/// <summary>
/// A special type of pager that allows for mutating the data.
/// </summary>
public class MutablePager<TKey, TItem>(IDataSource<TKey, TItem> dataSource, int pageSize = 20, bool supportsPrepending = false)
    : Pager<TKey, TItem>(dataSource, pageSize, supportsPrepending)
{
    /// <summary>
    /// Adds an item to the end of the data list.
    /// </summary>
    public void Add(TItem item)
    {
        _items.Add(item);
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Add, item);
        OnCollectionChanged(args);
    }

    /// <summary>
    /// Removes an item from the data.
    /// </summary>
    public void Remove(TItem item)
    {
        if (_items.Remove(item))
        {
            NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Remove, item);
            OnCollectionChanged(args);
        }
    }

    /// <summary>
    /// Clears the data.
    /// </summary>
    public void Clear()
    {
        _items.Clear();
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Reset);
        OnCollectionChanged(args);
    }

    /// <summary>
    /// Inserts an item at the specified index.
    /// </summary>
    public void Insert(int index, TItem item)
    {
        _items.Insert(index, item);
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Add, item, index);
        OnCollectionChanged(args);
    }

    /// <summary>
    /// Removes an item at the specified index.
    /// </summary>
    public void RemoveAt(int index)
    {
        var item = _items[index];
        _items.RemoveAt(index);
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Remove, item, index);
        OnCollectionChanged(args);
    }

    /// <summary>
    /// Moves an item from one index to another.
    /// </summary>
    public void Move(int oldIndex, int newIndex)
    {
        var item = _items[oldIndex];
        _items.RemoveAt(oldIndex);
        _items.Insert(newIndex, item);
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Move, item, oldIndex, newIndex);
        OnCollectionChanged(args);
    }

    /// <summary>
    /// Replaces an item at the specified index with a new item.
    /// </summary>
    public void Replace(int index, TItem item)
    {
        var oldItem = _items[index];
        _items[index] = item;
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Replace, item, oldItem, index);
        OnCollectionChanged(args);
    }

    /// <summary>
    /// Finds the index of the specified item.
    /// </summary>
    public int IndexOf(TItem item)
    {
        return _items.IndexOf(item);
    }

    /// <summary>
    /// Finds the index of the first item that matches the specified predicate.
    /// </summary>
    public int FindIndex(Predicate<TItem> match)
    {
        return _items.FindIndex(match);
    }

    /// <summary>
    /// Adds a range of items to the end of the data list.
    /// </summary>
    public void AddRange(IEnumerable<TItem> items)
    {
        _items.AddRange(items);
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Add, items);
        OnCollectionChanged(args);
    }

    /// <summary>
    /// Removes a range of items from the data list.
    /// </summary>
    public void RemoveRange(IEnumerable<TItem> items)
    {
        foreach (var item in items)
        {
            _items.Remove(item);
        }
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Remove, items);
        OnCollectionChanged(args);
    }

    /// <summary>
    /// Resets the data list with a new set of items.
    /// </summary>
    public void Reset(IEnumerable<TItem> items)
    {
        _items.Clear();
        _items.AddRange(items);
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Reset);
        OnCollectionChanged(args);
    }
}
