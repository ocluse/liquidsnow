using System.Reflection;
using System.Collections.Concurrent;
using Ocluse.LiquidSnow.Utils;

namespace Ocluse.LiquidSnow.Jobs.Internal;

internal sealed class JobDescriptorCache
{
    private readonly ConcurrentDictionary<string, JobDescriptor> _descriptors = [];

    private const string HandleMethodName = nameof(IJobHandler<IJob>.HandleAsync);

    public JobDescriptor GetDescriptor(Type jobType)
    {
        string key = CacheKeyHelper.GetKey(jobType);

        return _descriptors.GetOrAdd(key, (_) =>
        {
            Type handlerType = typeof(IJobHandler<>).MakeGenericType(jobType);

            Type[] paramTypes = [jobType, typeof(long), typeof(CancellationToken)];

            MethodInfo handleMethodInfo = handlerType.GetMethod(HandleMethodName, paramTypes)
                ?? throw new InvalidOperationException("Handle method not found on job handler");

            return new JobDescriptor(jobType, handlerType, handleMethodInfo);
        });
    }
}
