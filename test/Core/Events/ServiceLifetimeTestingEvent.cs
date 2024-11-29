using Ocluse.LiquidSnow.Events;

namespace Ocluse.LiquidSnow.Core.Tests.Events;

public record ServiceLifetimeTestingEvent
{
    public const int DELAY_MILI = 100;

    public bool Success { get; set; }
}

public sealed class DisposableService : IDisposable
{
    public bool Disposed { get; private set; }

    public void Dispose()
    {
        Disposed = true;
    }

    public async Task DoSomethingAsync()
    {
        ObjectDisposedException.ThrowIf(Disposed, this);

        await Task.Delay(ServiceLifetimeTestingEvent.DELAY_MILI);
    }
}

public sealed class ServiceLifetimeTestingEventListener(DisposableService disposableService)
    : IEventListener<ServiceLifetimeTestingEvent>
{
    public async Task HandleAsync(ServiceLifetimeTestingEvent e, CancellationToken cancellationToken = default)
    {
        await disposableService.DoSomethingAsync();
        e.Success = true;
    }
}