using System.Reflection;

namespace Ocluse.LiquidSnow.Jobs.Internal;

internal sealed record JobDescriptor
{
    public Type JobType { get; }
    public Type HandlerType { get; }
    public MethodInfo HandleMethodInfo { get; }

    public JobDescriptor(Type jobType, Type handlerType, MethodInfo handleMethodInfo)
    {
        JobType = jobType;
        HandlerType = handlerType;
        HandleMethodInfo = handleMethodInfo;
    }
}
