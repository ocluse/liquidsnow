using Ocluse.LiquidSnow.Utils;
using System.Collections.Concurrent;
using System.Reflection;

namespace Ocluse.LiquidSnow.Requests.Internal;

internal class RequestDescriptorCache
{
    private readonly ConcurrentDictionary<string, RequestDescriptor> _requestDescriptors = [];

    public RequestDescriptor GetDescriptor(Type requestType)
    {
        string key = CacheKeyHelper.GetKey(requestType);

        return _requestDescriptors.GetOrAdd(key, (_) =>
        {
            Type handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
            Type[] paramTypes = [requestType, typeof(CancellationToken)];

            MethodInfo methodInfo = handlerType.GetMethod(nameof(IRequestHandler<object>.HandleAsync), paramTypes)
                ?? throw new InvalidOperationException("Handle method not found on request handler");

            return new RequestDescriptor(requestType, handlerType, methodInfo);
        });
    }

    public RequestDescriptor GetDescriptor(Type requestType, Type resultType)
    {
        string key = CacheKeyHelper.GetKey(requestType, resultType);

        return _requestDescriptors.GetOrAdd(key, (_) =>
        {
            Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, resultType);
            Type[] paramTypes = [requestType, typeof(CancellationToken)];

            MethodInfo methodInfo = handlerType.GetMethod(nameof(IRequestHandler<object, object>.HandleAsync), paramTypes)
                ?? throw new InvalidOperationException("Handle method not found on request handler");

            return new RequestDescriptor(requestType, handlerType, methodInfo);
        });
    }
}
