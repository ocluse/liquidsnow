using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace Ocluse.LiquidSnow.Events.Internal;

internal sealed class EventBus(IServiceProvider serviceProvider) : IEventBus
{
    private static async Task ExecuteHandler(object? handler, MethodInfo handleMethodInfo, object[] handleMethodArgs)
    {
        if (handler == null)
        {
            return;
        }

        await (Task)handleMethodInfo.Invoke(handler, handleMethodArgs)!;
    }

    private static async Task PublishAsync<TEvent>(IServiceProvider serviceProvider, TEvent e, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(e, nameof(e));

        EventDescriptorCache descriptorCache = serviceProvider.GetRequiredService<EventDescriptorCache>();

        EventDescriptor descriptor = descriptorCache.GetDescriptor(typeof(TEvent));

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

    public void Publish<TEvent>(TEvent e, CancellationToken cancellationToken = default)
    {
        _ = Task.Run(async () =>
        {
            try
            {
                using IServiceScope scope = serviceProvider.CreateScope();
                await PublishAsync(scope.ServiceProvider, e, cancellationToken);
            }
            catch(Exception ex)
            {
                EventBusModel.OnUnobservedException(ex);
            }
           
        }, cancellationToken);
    }

    public async Task PublishAsync<TEvent>(TEvent e, CancellationToken cancellationToken = default)
    {
        await PublishAsync(serviceProvider, e, cancellationToken);
    }
}
