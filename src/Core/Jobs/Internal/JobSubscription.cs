namespace Ocluse.LiquidSnow.Jobs.Internal
{
    internal record JobSubscription(IDisposable Handle, IJob Job, CancellationTokenSource CancellationTokenSource) : IDisposable
    {
        public void Dispose()
        {
            Handle.Dispose();
            CancellationTokenSource.Cancel();
        }
    }
}
