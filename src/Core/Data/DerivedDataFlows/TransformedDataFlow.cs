namespace Ocluse.LiquidSnow.Data.DerivedDataFlows;

internal sealed class TransformedDataFlow<T, TResult>(IDataFlow<T> upstream, Func<T, TResult> selector) : IDataFlow<TResult>
{
    public bool Paused => upstream.Paused;

    public IDisposable Subscribe(Func<TResult, Task> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(
            value => subscriberFunc(selector(value)),
            bufferSize,
            overflowBehavior);
    }

    public IDisposable Subscribe(Action<TResult> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(
            value => subscriberFunc(selector(value)),
            bufferSize,
            overflowBehavior);
    }
}
