using Ocluse.LiquidSnow.Utils;
using System.Collections.Concurrent;
using System.Reflection;

namespace Ocluse.LiquidSnow.Events.Internal;

internal sealed class EventDescriptorCache
{
    private readonly ConcurrentDictionary<string, EventDescriptor> _descriptors = [];
    
    public EventDescriptor GetDescriptor(Type eventType)
    {
        string key = CacheKeyHelper.GetKey(eventType);
        
        return _descriptors.GetOrAdd(key, (_) =>
        {
            Type handlerType = typeof(IEventListener<>).MakeGenericType(eventType);
            Type[] paramTypes = [eventType, typeof(CancellationToken)];

            MethodInfo handleMethodInfo = handlerType.GetMethod(nameof(IEventListener<object>.HandleAsync), paramTypes)
                ?? throw new InvalidOperationException("Handle method not found on event handler");

            return new EventDescriptor(eventType, handlerType, handleMethodInfo);
        });
    }
}
