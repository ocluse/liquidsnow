namespace Ocluse.LiquidSnow.Cqrs.Internal;

internal sealed class CoreDispatcher(ExecutionDescriptorCache descriptorCache)
{
    public async Task<TExecutionResult> DispatchAsync<TExecutionResult>(
        ExecutionKind kind, 
        Type executionType, 
        object execution, 
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken)
    {
        ExecutionDescriptor[] chain = descriptorCache.GetPolymorphicChain(kind, executionType, typeof(TExecutionResult));

        //Preprocess:
        var (preprocessor, preprocessorDescriptor) = Resolve(chain, d => d.PreprocessorType, serviceProvider);
        
        if (preprocessor != null)
        {
            var task = (Task)preprocessorDescriptor!.PreprocessMethodInfo.Invoke(preprocessor, [execution, cancellationToken])!;
            await task;
            execution = preprocessorDescriptor.TaskResultPropertyInfo.GetValue(task)!;
        }

        //Handle:
        var (handler, handlerDescriptor) = Resolve(chain, d => d.HandlerType, serviceProvider);

        if (handler == null)
        {
            throw new InvalidOperationException(
                $"No handler registered for '{executionType.Name}' or any of its base types (in case of polymorphic resolution).");
        }

        TExecutionResult result = await (Task<TExecutionResult>)handlerDescriptor!.HandleMethodInfo.Invoke(handler, [execution, cancellationToken])!;

        //Postprocess:
        var (postprocessor, postprocessorDescriptor) = Resolve(chain, d => d.PostprocessorType, serviceProvider);

        if (postprocessor != null)
        {
            result = await (Task<TExecutionResult>)postprocessorDescriptor!.PostprocessMethodInfo.Invoke(postprocessor, [execution, result!, cancellationToken])!;
        }

        return result;
    }

    private static (object? Service, ExecutionDescriptor? Descriptor) Resolve(
    ExecutionDescriptor[] chain,
    Func<ExecutionDescriptor, Type> typeSelector,
    IServiceProvider serviceProvider)
    {
        foreach (ExecutionDescriptor descriptor in chain)
        {
            object? service = serviceProvider.GetService(typeSelector(descriptor));
            if (service != null)
                return (service, descriptor);
        }
        return (null, null);
    }
}
