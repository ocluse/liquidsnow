namespace Ocluse.LiquidSnow.Data.DerivedDataFlows;

internal sealed class FilteredDataFlow<T>(IDataFlow<T> upstream, Func<T, bool> predicate) : IDataFlow<T>
{
    public bool Paused => upstream.Paused;

    public IDisposable Subscribe(Func<T, Task> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(
            value => predicate(value) ? subscriberFunc(value) : Task.CompletedTask,
            bufferSize,
            overflowBehavior);
    }

    public IDisposable Subscribe(Action<T> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(
            value => { if (predicate(value)) subscriberFunc(value); },
            bufferSize,
            overflowBehavior);
    }
}
