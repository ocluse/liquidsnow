using Ocluse.LiquidSnow.Utils;
using System.Collections.Concurrent;

namespace Ocluse.LiquidSnow.Cqrs.Internal;

internal sealed class ExecutionDescriptorCache
{
    private readonly ConcurrentDictionary<string, ExecutionDescriptor> _descriptors
        = new();

    public ExecutionDescriptor GetDescriptor(ExecutionKind kind, Type executionType, Type resultType)
    {
        string key = CacheKeyHelper.GetKey(executionType, resultType) + $"::{kind}";

        return _descriptors.GetOrAdd(key, _ =>
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
        });
    }
}
