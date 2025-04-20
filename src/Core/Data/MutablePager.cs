using System.Collections.Specialized;

namespace Ocluse.LiquidSnow.Data;

public class MutablePager<TKey, TItem>(IDataSource<TKey, TItem> dataSource, int pageSize = 20, bool supportsPrepending = false)
    : Pager<TKey, TItem>(dataSource, pageSize, supportsPrepending)
{
    public void Add(TItem item)
    {
        _items.Add(item);
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Add, item);
        OnCollectionChanged(args);
    }

    public void Remove(TItem item)
    {
        if (_items.Remove(item))
        {
            NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Remove, item);
            OnCollectionChanged(args);
        }
    }

    public void Clear()
    {
        _items.Clear();
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Reset);
        OnCollectionChanged(args);
    }

    public void Insert(int index, TItem item)
    {
        _items.Insert(index, item);
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Add, item, index);
        OnCollectionChanged(args);
    }

    public void RemoveAt(int index)
    {
        var item = _items[index];
        _items.RemoveAt(index);
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Remove, item, index);
        OnCollectionChanged(args);
    }

    public void Move(int oldIndex, int newIndex)
    {
        var item = _items[oldIndex];
        _items.RemoveAt(oldIndex);
        _items.Insert(newIndex, item);
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Move, item, oldIndex, newIndex);
        OnCollectionChanged(args);
    }

    public void Replace(int index, TItem item)
    {
        var oldItem = _items[index];
        _items[index] = item;
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Replace, item, oldItem, index);
        OnCollectionChanged(args);
    }

    public int IndexOf(TItem item)
    {
        return _items.IndexOf(item);
    }

    public int FindIndex(Predicate<TItem> match)
    {
        return _items.FindIndex(match);
    }

    public void AddRange(IEnumerable<TItem> items)
    {
        _items.AddRange(items);
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Add, items);
        OnCollectionChanged(args);
    }

    public void RemoveRange(IEnumerable<TItem> items)
    {
        foreach (var item in items)
        {
            _items.Remove(item);
        }
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Remove, items);
        OnCollectionChanged(args);
    }

    public void Reset(IEnumerable<TItem> items)
    {
        _items.Clear();
        _items.AddRange(items);
        NotifyCollectionChangedEventArgs args = new(NotifyCollectionChangedAction.Reset);
        OnCollectionChanged(args);
    }
}
