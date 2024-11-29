using System.Reflection;

namespace Ocluse.LiquidSnow.Cqrs.Internal;

internal sealed record ExecutionDescriptor
{
    public const string HANDLE_METHOD_NAME = nameof(ICommandHandler<ICommand>.HandleAsync);
    public const string E_HANLE_METHOD_NOT_FOUND = "Handle method not found on handler";
    public const string E_VALUE_PROPERTY_NOT_FOUND = "'Result' property not found on the Task result type";
    
    public ExecutionDescriptor(Type executionType, Type resultType, Type handlerType, Type preprocessorType, Type postprocessorType)
    {
        ExecutionType = executionType;
        ResultType = resultType;
        HandlerType = handlerType;
        PreprocessorType = preprocessorType;
        PostprocessorType = postprocessorType;

        Type[] processParamTypes = [executionType, typeof(CancellationToken)];
        Type[] postprocessParamTypes = [executionType, resultType, typeof(CancellationToken)];

        PreprocessMethodInfo = PreprocessorType.GetMethod(HANDLE_METHOD_NAME, processParamTypes)
            ?? throw new InvalidOperationException(E_HANLE_METHOD_NOT_FOUND);

        HandleMethodInfo = HandlerType.GetMethod(HANDLE_METHOD_NAME, processParamTypes)
            ?? throw new InvalidOperationException(E_HANLE_METHOD_NOT_FOUND);

        PostprocessMethodInfo = PostprocessorType.GetMethod(HANDLE_METHOD_NAME, postprocessParamTypes)
            ?? throw new InvalidOperationException(E_HANLE_METHOD_NOT_FOUND);

        TaskResultPropertyInfo = typeof(Task<>).MakeGenericType(executionType).GetProperty(nameof(Task<object>.Result))
            ?? throw new InvalidOperationException(E_VALUE_PROPERTY_NOT_FOUND);
    }

    public Type ExecutionType { get; }

    public Type ResultType { get; }

    public Type HandlerType { get; }

    public Type PreprocessorType { get; }

    public Type PostprocessorType { get; }

    public MethodInfo PreprocessMethodInfo { get; } 

    public MethodInfo HandleMethodInfo { get; }

    public MethodInfo PostprocessMethodInfo { get; }

    public PropertyInfo TaskResultPropertyInfo { get; }
}
