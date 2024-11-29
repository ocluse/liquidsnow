using System.Reflection;

namespace Ocluse.LiquidSnow.Requests.Internal;

internal sealed record RequestDescriptor(Type EventType, Type HandlerType, MethodInfo MethodInfo);
