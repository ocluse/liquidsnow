using Microsoft.Extensions.DependencyInjection;

namespace Ocluse.LiquidSnow.Events.Internal;

internal sealed class EventBus(
    IServiceProvider serviceProvider,
    IServiceScopeFactory serviceScopeFactory,
    IEnumerable<IEventDispatchContributor> contributors)
    : IEventBus
{
    private readonly Dictionary<EventDispatchKey, EventDispatchDescriptor> _descriptors = contributors
        .SelectMany(x => x.Descriptors)
        .GroupBy(x => new EventDispatchKey(x.EventType))
        .ToDictionary(x => x.Key, x => x.Single());

    private static async Task PublishAsync(IServiceProvider serviceProvider, object e, Type eventType, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(e, nameof(e));

        EventBus eventBus = serviceProvider.GetRequiredService<EventBus>();
        await eventBus.PublishInternalAsync(serviceProvider, e, eventType, cancellationToken);
    }

    private Task PublishInternalAsync(
        IServiceProvider serviceProvider,
        object e,
        Type eventType,
        CancellationToken cancellationToken)
    {
        var key = new EventDispatchKey(eventType);
        if (!_descriptors.TryGetValue(key, out EventDispatchDescriptor? descriptor))
        {
            return Task.CompletedTask;
        }

        return descriptor.ExecuteAsync(e, serviceProvider, cancellationToken);
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
        await PublishInternalAsync(serviceProvider, e, eventType, cancellationToken);
    }
}
