using Microsoft.Extensions.DependencyInjection;

namespace Ocluse.LiquidSnow.Events;

/// <summary>
/// Provides strongly typed event execution helpers used by generated dispatch code.
/// </summary>
public static class EventDispatchExecutor
{
    /// <summary>
    /// Executes all listeners registered for <typeparamref name="TEvent"/>.
    /// </summary>
    public static async Task ExecuteAsync<TEvent>(
        object e,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
    {
        TEvent typedEvent = (TEvent)e;

        IEnumerable<IEventListener<TEvent>> listeners = serviceProvider.GetServices<IEventListener<TEvent>>();

        List<Exception> exceptions = [];

        foreach (IEventListener<TEvent> listener in listeners)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await listener.HandleAsync(typedEvent, cancellationToken);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count == 1)
        {
            throw exceptions[0];
        }

        if (exceptions.Count > 1)
        {
            throw new AggregateException(exceptions);
        }
    }
}
