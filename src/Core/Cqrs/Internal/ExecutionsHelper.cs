using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Cqrs.Internal
{
    internal enum ExecutionKind
    {
        Command,
        Query
    }

    internal static class ExecutionsHelper
    {
        private static readonly Dictionary<string, ExecutionDescriptor> _descriptorCache
            = new Dictionary<string, ExecutionDescriptor>();

        private static ExecutionDescriptor CreateDescriptor<TResult>(ExecutionKind kind, Type executionType)
        {
            Type preExecutor = ExecutionKind.Command == kind
                ? typeof(IPreCommandExecutionHandler<,>)
                : typeof(IPreQueryExecutionHandler<,>);

            Type handlerType = ExecutionKind.Command == kind
                ? typeof(ICommandHandler<,>)
                : typeof(IQueryHandler<,>);

            Type postExecutor = ExecutionKind.Command == kind
                ? typeof(IPostCommandExecutionHandler<,>)
                : typeof(IPostQueryExecutionHandler<,>);

            Type resultType = typeof(TResult);

            Type preExecutionHandler = preExecutor.MakeGenericType(executionType, resultType);

            Type postExecutionHandler = postExecutor.MakeGenericType(executionType, resultType);

            Type executionHandler = handlerType.MakeGenericType(executionType, resultType);

            return new ExecutionDescriptor(preExecutionHandler, postExecutionHandler, executionHandler, executionType, resultType);
        }

        public static ExecutionDescriptor GetDescriptor<TResult>(ExecutionKind kind, object execution)
        {
            Type executionType = execution.GetType();

            string key = $"{executionType.FullName}_{kind}";

            if (_descriptorCache.TryGetValue(key, out ExecutionDescriptor descriptor))
            {
                return descriptor;
            }
            else
            {
                descriptor = CreateDescriptor<TResult>(kind, executionType);

                try
                {
                    _descriptorCache.Add(key, descriptor);
                }
                catch (ArgumentException)
                {
                    //do nothing
                }

                return descriptor;
            }
        }

        public static async Task<TResult> ExecuteDescriptor<TResult>(object execution, ExecutionDescriptor descriptor, IServiceProvider serviceProvider, CancellationToken cancellationToken)
        {
            //do we have a pre execution handler?
            var preExecutionHandler = serviceProvider.GetService(descriptor.PreExecutionHandler);

            if (preExecutionHandler != null)
            {
                var preExecutionResult = await (Task<PreExecutionResult>)descriptor.PreExecute.Invoke(preExecutionHandler, new object[] { execution, cancellationToken });

                var preExecutionResultType = preExecutionResult.GetType();

                if (preExecutionResultType == descriptor.StopExecutionResultType)
                {
                    return (TResult)descriptor.StopExecutionResultValuePropertyInfo!.GetValue(preExecutionResult);
                }
                else if (preExecutionResultType != typeof(PreExecutionResult.ContinuePreExecutionResult))
                {
                    throw new InvalidOperationException("Invalid pre execution result");
                }
            }

            //get the execution handler
            var executionHandler = serviceProvider.GetService(descriptor.ExecutionHandler)
                ?? throw new InvalidOperationException($"No handler has been registered for the type {descriptor.ExecutionType.Name}");

            //execute the handler
            TResult result = await (Task<TResult>)descriptor.Execute.Invoke(executionHandler, new object[] { execution, cancellationToken });

            //do we have a post execution handler?
            var postExecutionHandler = serviceProvider.GetService(descriptor.PostExecutionHandler);
            if (postExecutionHandler != null)
            {
                result = await (Task<TResult>)descriptor.PostExecute.Invoke(postExecutionHandler, new object[] { execution, result!, cancellationToken });
            }

            return result;
        }
    }
}
