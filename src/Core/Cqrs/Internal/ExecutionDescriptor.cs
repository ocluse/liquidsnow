using System;
using System.Reflection;
using System.Threading;

namespace Ocluse.LiquidSnow.Cqrs.Internal
{
    internal readonly struct ExecutionDescriptor
    {
        //private readonly Type? _stopExecutionResultType;

        //private readonly MethodInfo? _preExecute, _postExecute, _execute;

        //private readonly PropertyInfo? _stopExecutionResultValuePropertyInfo;

        public const string HANDLE_METHOD_NAME = "Handle";

        public ExecutionDescriptor(Type preExecutionHandler, Type postExecutionHandler, Type executionHandler, Type executionType, Type resultType)
        {
            PreExecutionHandler = preExecutionHandler;
            PostExecutionHandler = postExecutionHandler;
            ExecutionHandler = executionHandler;
            ResultType = resultType;
            ExecutionType = executionType;

            StopExecutionResultType = typeof(PreExecutionResult.StopPreExecutionResult<>).MakeGenericType(ResultType);

            var paramTypes = new Type[] { ExecutionType, typeof(CancellationToken) };
            
            PreExecute = PreExecutionHandler.GetMethod(HANDLE_METHOD_NAME, paramTypes)
                ?? throw new InvalidOperationException("Handle method not found on handler");

            Execute = ExecutionHandler.GetMethod(HANDLE_METHOD_NAME, paramTypes)
                        ?? throw new InvalidOperationException("Handle method not found on handler");

            var postParamTypes = new Type[] { ExecutionType, ResultType, typeof(CancellationToken) };

            PostExecute = PostExecutionHandler.GetMethod(HANDLE_METHOD_NAME, postParamTypes)
                        ?? throw new InvalidOperationException("Handle method not found on handler");

            StopExecutionResultType = typeof(PreExecutionResult.StopPreExecutionResult<>).MakeGenericType(ResultType);

            StopExecutionResultValuePropertyInfo = StopExecutionResultType.GetProperty("Value")
                        ?? throw new InvalidOperationException("Value property not found on stop execution result");
        }

        public Type ExecutionType { get; }

        public Type PreExecutionHandler { get; }

        public Type PostExecutionHandler { get; }

        public Type ExecutionHandler { get; }

        public Type StopExecutionResultType{ get; }

        public PropertyInfo StopExecutionResultValuePropertyInfo { get; }

        public Type ResultType { get; }

        public MethodInfo PreExecute { get; }

        public MethodInfo Execute{get;}

        public MethodInfo PostExecute { get; }
    }
}
