using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ocluse.LiquidSnow.Events.Internal;

internal sealed class EventBus(IServiceProvider serviceProvider, IServiceScopeFactory serviceScopeFactory) : IEventBus
{
    private static async Task ExecuteHandler(object? handler, MethodInfo handleMethodInfo, object[] handleMethodArgs)
    {
        if (handler == null)
        {
            return;
        }

        await (Task)handleMethodInfo.Invoke(handler, handleMethodArgs)!;
    }

    private static async Task PublishAsync(IServiceProvider serviceProvider, object e, Type eventType, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(e, nameof(e));

        EventDescriptorCache descriptorCache = serviceProvider.GetRequiredService<EventDescriptorCache>();

        EventDescriptor descriptor = descriptorCache.GetDescriptor(eventType);

        object[] handleMethodArgs = [e, cancellationToken];

        IEnumerable<object?> handlers = serviceProvider
            .GetServices(descriptor.HandlerType);

        List<Exception> exceptions = [];

        foreach (object? handler in handlers)
        {
            cancellationToken.ThrowIfCancellationRequested();

            try
            {
                await ExecuteHandler(handler, descriptor.HandleMethodInfo, handleMethodArgs);
            }
            catch (Exception ex)
            {
                exceptions.Add(ex);
            }
        }

        if (exceptions.Count > 1)
        {
            throw new AggregateException(exceptions);
        }
    }

    public void Publish<TEvent>(TEvent e)
    {
        ArgumentNullException.ThrowIfNull(e, nameof(e));
        Publish(e, typeof(TEvent));
    }

    public void Publish(object e, Type eventType)
    {
        ArgumentNullException.ThrowIfNull(e, nameof(e));
        _ = Task.Run(async () =>
        {
            try
            {
                using IServiceScope scope = serviceScopeFactory.CreateScope();
                await PublishAsync(scope.ServiceProvider, e, eventType);
            }
            catch (Exception ex)
            {
                EventBusModel.OnUnobservedException(ex);
            }

        });
    }

    public async Task PublishAsync<TEvent>(TEvent e, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(e, nameof(e));
        await PublishAsync(e, typeof(TEvent), cancellationToken);
    }

    public async Task PublishAsync(object e, Type eventType, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(e, nameof(e));
        await PublishAsync(serviceProvider, e, eventType, cancellationToken);
    }
}
