using Ocluse.LiquidSnow.Extensions;
using Ocluse.LiquidSnow.Utils;
using System.Collections.Concurrent;
using System.Reflection;

namespace Ocluse.LiquidSnow.Requests.Internal;

internal class RequestDescriptorCache(RequestsOptions options)
{
    private readonly ConcurrentDictionary<string, RequestDescriptor[]> _polymorphicChains = [];

    public RequestDescriptor[] GetPolymorphicChain(Type requestType)
    {
        string key = CacheKeyHelper.GetKey(requestType);
        return _polymorphicChains.GetOrAdd(key, (_) =>
        {
            List<RequestDescriptor> chain = [CreateDescriptor(requestType)];

            if (options.EnablePolymorphicResolution)
            {
                foreach(var baseType in requestType.GetBaseTypes())
                {
                    chain.Add(CreateDescriptor(baseType));
                }
            }

            return [.. chain];
        });
    }

    public RequestDescriptor[] GetPolymorphicChain(Type requestType, Type resultType)
    {
        string key = CacheKeyHelper.GetKey(requestType, resultType);
        return _polymorphicChains.GetOrAdd(key, (_) =>
        {
            List<RequestDescriptor> chain = [CreateDescriptor(requestType, resultType)];

            if (options.EnablePolymorphicResolution)
            {
                foreach (var baseType in requestType.GetBaseTypes())
                {
                    chain.Add(CreateDescriptor(baseType, resultType));
                }
            }

            return [.. chain];
        });
    }

    public RequestDescriptor CreateDescriptor(Type requestType)
    {
        string key = CacheKeyHelper.GetKey(requestType);

        Type handlerType = typeof(IRequestHandler<>).MakeGenericType(requestType);
        Type[] paramTypes = [requestType, typeof(CancellationToken)];

        MethodInfo methodInfo = handlerType.GetMethod(nameof(IRequestHandler<object>.HandleAsync), paramTypes)
            ?? throw new InvalidOperationException("Handle method not found on request handler");

        return new RequestDescriptor(requestType, handlerType, methodInfo);
    }

    public RequestDescriptor CreateDescriptor(Type requestType, Type resultType)
    {
        string key = CacheKeyHelper.GetKey(requestType, resultType);

        Type handlerType = typeof(IRequestHandler<,>).MakeGenericType(requestType, resultType);
        Type[] paramTypes = [requestType, typeof(CancellationToken)];

        MethodInfo methodInfo = handlerType.GetMethod(nameof(IRequestHandler<object, object>.HandleAsync), paramTypes)
            ?? throw new InvalidOperationException("Handle method not found on request handler");

        return new RequestDescriptor(requestType, handlerType, methodInfo);
    }
}
