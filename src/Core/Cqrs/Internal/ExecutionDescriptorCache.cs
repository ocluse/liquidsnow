using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Utils;
using System.Collections.Concurrent;

namespace Ocluse.LiquidSnow.Cqrs.Internal;

internal sealed class ExecutionDescriptorCache(CqrsOptions options)
{
    private readonly ConcurrentDictionary<string, ExecutionDescriptor[]> _polymorphicChains
        = new();

    public ExecutionDescriptor[] GetPolymorphicChain(ExecutionKind kind, Type executionType, Type resultType)
    {
        string key = CacheKeyHelper.GetKey(executionType, resultType) + $"::{kind}";

        return _polymorphicChains.GetOrAdd(key, _ =>
        {
            List<ExecutionDescriptor> chain = [CreateDescriptor(kind, executionType, resultType)];

            if (options.EnablePolymorphicResolution)
            {
                foreach(var baseExecutionType in executionType.GetBaseTypes())
                {
                    chain.Add(CreateDescriptor(kind, baseExecutionType, resultType));
                }
            }

            return [.. chain];
        });
    }

    private static ExecutionDescriptor CreateDescriptor(ExecutionKind kind, Type executionType, Type resultType)
    {
        Type[] genericParamTypes = [executionType, resultType];

        Type preprocessorType = (ExecutionKind.Command == kind
            ? typeof(ICommandPreprocessor<,>)
            : typeof(IQueryPreprocessor<,>)).MakeGenericType(genericParamTypes);

        Type handlerType = (ExecutionKind.Command == kind
            ? typeof(ICommandHandler<,>)
            : typeof(IQueryHandler<,>)).MakeGenericType(genericParamTypes);

        Type postprocessorType = (ExecutionKind.Command == kind
            ? typeof(ICommandPostprocessor<,>)
            : typeof(IQueryPostprocessor<,>)).MakeGenericType(genericParamTypes);

        return new ExecutionDescriptor(executionType, resultType, handlerType, preprocessorType, postprocessorType);
    }
}
