namespace Ocluse.LiquidSnow.Data.DerivedDataFlows;

internal sealed class DistinctUntilChangedDataFlow<T>(IDataFlow<T> upstream, IEqualityComparer<T> comparer) : IDataFlow<T>
{
    public bool Paused => upstream.Paused;

    public IDisposable Subscribe(Func<T, Task> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(BuildDistinct(subscriberFunc), bufferSize, overflowBehavior);
    }

    public IDisposable Subscribe(Action<T> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(
            BuildDistinct(value => { subscriberFunc(value); return Task.CompletedTask; }),
            bufferSize,
            overflowBehavior);
    }

    private Func<T, Task> BuildDistinct(Func<T, Task> invoke)
    {
        T lastValue = default!;
        bool hasValue = false;

        return value =>
        {
            if (hasValue && comparer.Equals(value, lastValue))
                return Task.CompletedTask;

            lastValue = value;
            hasValue = true;
            return invoke(value);
        };
    }
}
