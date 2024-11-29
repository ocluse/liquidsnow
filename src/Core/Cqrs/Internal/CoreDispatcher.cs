using Microsoft.Extensions.DependencyInjection;

namespace Ocluse.LiquidSnow.Cqrs.Internal;

internal sealed class CoreDispatcher(ExecutionDescriptorCache descriptorCache)
{
    public async Task<TExecutionResult> DispatchAsync<TExecutionResult>(ExecutionKind kind, Type executionType, object execution, IServiceProvider serviceProvider, CancellationToken cancellationToken)
    {
        ExecutionDescriptor descriptor = descriptorCache.GetDescriptor(kind, executionType, typeof(TExecutionResult));

        object? preprocessor = serviceProvider.GetService(descriptor.PreprocessorType);

        if (preprocessor != null)
        {
            var task = (Task)descriptor.PreprocessMethodInfo.Invoke(preprocessor, [execution, cancellationToken])!;
            await task;
            execution = descriptor.TaskResultPropertyInfo.GetValue(task)!;
            //execution = await (dynamic)descriptor.Preprocess.Invoke(preprocessor, [execution, cancellationToken])!;
        }

        //Handler:
        object handler = serviceProvider.GetRequiredService(descriptor.HandlerType);

        TExecutionResult result = await (Task<TExecutionResult>)descriptor.HandleMethodInfo.Invoke(handler, [execution, cancellationToken])!;

        //Postprocessing:
        var postExecutionHandler = serviceProvider.GetService(descriptor.PostprocessorType);
        if (postExecutionHandler != null)
        {
            result = await (Task<TExecutionResult>)descriptor.PostprocessMethodInfo.Invoke(postExecutionHandler, [execution, result!, cancellationToken])!;
        }

        return result;
    }
}
