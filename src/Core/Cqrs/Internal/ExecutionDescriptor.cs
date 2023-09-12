using System;
using System.Reflection;
using System.Threading;

namespace Ocluse.LiquidSnow.Cqrs.Internal
{
    internal class ExecutionDescriptor
    {
        private Type? _stopExecutionResultType;

        private MethodInfo? _preExecute, _postExecute, _execute;

        private PropertyInfo? _stopExecutionResultValuePropertyInfo;

        public const string HANDLE_METHOD_NAME = "Handle";

        public ExecutionDescriptor(Type preExecutionHandler, Type postExecutionHandler, Type executionHandler, Type executionType, Type resultType)
        {
            PreExecutionHandler = preExecutionHandler;
            PostExecutionHandler = postExecutionHandler;
            ExecutionHandler = executionHandler;
            ResultType = resultType;
            ExecutionType = executionType;
        }

        public Type ExecutionType { get; }

        public Type PreExecutionHandler { get; }

        public Type PostExecutionHandler { get; }

        public Type ExecutionHandler { get; }

        public Type StopExecutionResultType
        {
            get
            {
                if (_stopExecutionResultType == null)
                {
                    _stopExecutionResultType = typeof(PreExecutionResult.StopPreExecutionResult<>).MakeGenericType(ResultType);
                }

                return _stopExecutionResultType;
            }

        }

        public PropertyInfo StopExecutionResultValuePropertyInfo
        {
            get
            {
                if (_stopExecutionResultValuePropertyInfo == null)
                {
                    _stopExecutionResultValuePropertyInfo = StopExecutionResultType.GetProperty("Value")
                        ?? throw new InvalidOperationException("Value property not found on stop execution result");
                }
                return _stopExecutionResultValuePropertyInfo;
            }
        }

        public Type ResultType { get; }

        public MethodInfo PreExecute
        {
            get
            {
                if (_preExecute == null)
                {
                    var paramTypes = new Type[] { ExecutionType, typeof(CancellationToken) };
                    _preExecute = PreExecutionHandler.GetMethod(HANDLE_METHOD_NAME, paramTypes)
                        ?? throw new InvalidOperationException("Handle method not found on handler");
                }

                return _preExecute;
            }
        }

        public MethodInfo Execute
        {
            get
            {
                if (_execute == null)
                {
                    var paramTypes = new Type[] { ExecutionType, typeof(CancellationToken) };

                    _execute = ExecutionHandler.GetMethod(HANDLE_METHOD_NAME, paramTypes)
                        ?? throw new InvalidOperationException("Handle method not found on handler");
                }
                return _execute;
            }
        }

        public MethodInfo PostExecute
        {
            get
            {
                if (_postExecute == null)
                {
                    var paramTypes = new Type[] { ExecutionType, ResultType, typeof(CancellationToken) };

                    _postExecute = PostExecutionHandler.GetMethod(HANDLE_METHOD_NAME, paramTypes)
                        ?? throw new InvalidOperationException("Handle method not found on handler");
                }
                return _postExecute;
            }
        }
    }
}
