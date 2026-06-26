using Ocluse.LiquidSnow.DependencyInjection;
using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Utils;
using System.Collections.Concurrent;
using System.Reflection;

namespace Ocluse.LiquidSnow.Events.Internal;

internal sealed class EventDescriptorCache(EventBusOptions eventBusOptions)
{
    private readonly ConcurrentDictionary<string, List<EventDescriptor>> _descriptors = [];

    public IEnumerable<EventDescriptor> GetDescriptors(Type eventType)
    {
        string key = CacheKeyHelper.GetKey(eventType);

        return _descriptors.GetOrAdd(key, (_) =>
        {
            List<EventDescriptor> descriptors = [CreateDescriptor(eventType)];

            if (eventBusOptions.EnablePolymorphicResolution || eventType.IsDefined(typeof(PolymorphicResolutionAttribute), false))
            {
                foreach (var baseType in eventType.GetBaseTypes())
                {
                    descriptors.Add(CreateDescriptor(baseType));
                }
            }
            return descriptors;
        });
    }

    private static EventDescriptor CreateDescriptor(Type eventType)
    {
        Type handlerType = typeof(IEventListener<>).MakeGenericType(eventType);
        Type[] paramTypes = [eventType, typeof(CancellationToken)];
        MethodInfo handleMethodInfo = handlerType.GetMethod(nameof(IEventListener<object>.HandleAsync), paramTypes)
            ?? throw new InvalidOperationException("Handle method not found on event handler of type " + handlerType.FullName);
        return new EventDescriptor(eventType, handlerType, handleMethodInfo);
    }
}
