using Ocluse.LiquidSnow.Utils;
using System.Collections.Concurrent;

namespace Ocluse.LiquidSnow.Orchestrations.Internal;

internal class OrchestrationDescriptorCache
{
    private readonly ConcurrentDictionary<string, OrchestrationDescriptor> cache = new();

    public OrchestrationDescriptor GetDescriptor(Type orchestrationType, Type resultType)
    {
        string key = CacheKeyHelper.GetKey(orchestrationType, resultType);
        
        return cache.GetOrAdd(key, (_) =>
        {
            return new OrchestrationDescriptor(orchestrationType, resultType);
        });
    }
}
