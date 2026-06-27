using Ocluse.LiquidSnow.DependencyInjection;
using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Utils;
using System.Collections.Concurrent;
using System.Reflection;

namespace Ocluse.LiquidSnow.Jobs.Internal;

internal sealed class JobDescriptorCache(JobsOptions options)
{
    private readonly ConcurrentDictionary<string, JobDescriptor[]> _polymorphicChains = [];

    private const string HandleMethodName = nameof(IJobHandler<IJob>.HandleAsync);

    public JobDescriptor[] GetPolymorphicChain(Type jobType)
    {
        string key = CacheKeyHelper.GetKey(jobType);

        return _polymorphicChains.GetOrAdd(key, (_) =>
        {
            List<JobDescriptor> chain = [CreateDescriptor(jobType)];

            if (options.EnablePolymorphicResolution || jobType.IsDefined(typeof(PolymorphicResolutionAttribute), false))
            {
                foreach(var baseType in jobType.GetBaseTypes())
                {
                    chain.Add(CreateDescriptor(baseType));
                }
            }

            return [.. chain];
        });
    }

    private static JobDescriptor CreateDescriptor(Type jobType)
    {
        Type handlerType = typeof(IJobHandler<>).MakeGenericType(jobType);

        Type[] paramTypes = [jobType, typeof(long), typeof(CancellationToken)];

        MethodInfo handleMethodInfo = handlerType.GetMethod(HandleMethodName, paramTypes)
            ?? throw new InvalidOperationException("Handle method not found on job handler");

        return new JobDescriptor(jobType, handlerType, handleMethodInfo);
    }
}
