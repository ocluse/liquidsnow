namespace Ocluse.LiquidSnow.Data.DerivedDataFlows;

internal sealed class DoDataFlow<T>(IDataFlow<T> upstream, Action<T> sideEffect) : IDataFlow<T>
{
    public bool Paused => upstream.Paused;

    public IDisposable Subscribe(Func<T, Task> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(value =>
        {
            sideEffect(value);
            return subscriberFunc(value);
        }, bufferSize, overflowBehavior);
    }

    public IDisposable Subscribe(Action<T> subscriberFunc, int bufferSize = 0, BufferOverflowBehavior overflowBehavior = BufferOverflowBehavior.DropOldest)
    {
        return upstream.Subscribe(value =>
        {
            sideEffect(value);
            subscriberFunc(value);
        }, bufferSize, overflowBehavior);
    }
}
