using System.Reflection;

namespace Ocluse.LiquidSnow.Events.Internal;

internal sealed record EventDescriptor(
    Type EventType, 
    Type HandlerType, 
    MethodInfo HandleMethodInfo);
