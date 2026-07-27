using Microsoft.Extensions.DependencyInjection;

namespace Ocluse.LiquidSnow.Requests.Internal;

internal class RequestDispatcher(RequestDescriptorCache descriptorCache, IServiceProvider serviceProvider) : IRequestDispatcher
{
    private async Task<TResult> DispatchAsync<TResult>(object request, Type requestType, CancellationToken cancellationToken)
    {
        Type resultType = typeof(TResult);
        RequestDescriptor[] chain = descriptorCache.GetPolymorphicChain(requestType);
        (var handler, var descriptor) = Resolve(chain, serviceProvider);

        if (handler == null || descriptor == null)
        {
            throw new InvalidOperationException($"No handler found for request type {requestType.FullName} and result type {resultType.FullName}");
        }

        object[] handleMethodArgs = [request, cancellationToken];

        return await (Task<TResult>)descriptor.MethodInfo.Invoke(handler, handleMethodArgs)!;
    }

    public async Task DispatchAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        Type requestType = typeof(TRequest);
        RequestDescriptor[] chain = descriptorCache.GetPolymorphicChain(requestType);
        (var handler, var descriptor) = Resolve(chain, serviceProvider);

        if (handler == null || descriptor == null)
        {
            throw new InvalidOperationException($"No handler found for request type {requestType.FullName}");
        }

        object[] handleMethodArgs = [request, cancellationToken];

        await (Task)descriptor.MethodInfo.Invoke(handler, handleMethodArgs)!;
    }

    public async Task<TResult> DispatchAsync<TResult>(object request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        Type requestType = request.GetType();
        return await DispatchAsync<TResult>(request, requestType, cancellationToken);
    }

    public async Task<TResult> DispatchAsync<TRequest, TResult>(TRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        Type requestType = typeof(TRequest);
        return await DispatchAsync<TResult>(request, requestType, cancellationToken);
    }

    public async Task<TResult> DispatchAsync<TResult>(Type requestType, object request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        return await DispatchAsync<TResult>(request, requestType, cancellationToken);
    }

    public async Task DispatchAsync(Type requestType, object request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));
        Type resultType = typeof(object);

        RequestDescriptor[] chain = descriptorCache.GetPolymorphicChain(requestType);
        (var handler, var descriptor) = Resolve(chain, serviceProvider);

        if (handler == null || descriptor == null)
        {
            throw new InvalidOperationException($"No handler found for request type {requestType.FullName}");
        }

        object[] handleMethodArgs = [request, cancellationToken];

        await (Task)descriptor.MethodInfo.Invoke(handler, handleMethodArgs)!;
    }

    private static (object? Service, RequestDescriptor? Descriptor) Resolve(
        RequestDescriptor[] chain,
        IServiceProvider serviceProvider)
    {
        foreach (RequestDescriptor descriptor in chain)
        {
            object? service = serviceProvider.GetService(descriptor.HandlerType);
            if (service != null)
                return (service, descriptor);
        }
        return (null, null);
    }
}
