using Microsoft.Extensions.DependencyInjection;

namespace Ocluse.LiquidSnow.Requests.Internal;

internal class RequestDispatcher(RequestDescriptorCache descriptorCache, IServiceProvider serviceProvider) : IRequestDispatcher
{
    private async Task<TResult> DispatchAsync<TResult>(object request, Type requestType,  CancellationToken cancellationToken)
    {
        Type resultType = typeof(TResult);

        RequestDescriptor requestDescriptor = descriptorCache.GetDescriptor(requestType, resultType);

        object requestHandler = serviceProvider.GetRequiredService(requestDescriptor.HandlerType);

        object[] handleMethodArgs = [request, cancellationToken];

        return await (Task<TResult>)requestDescriptor.MethodInfo.Invoke(requestHandler, handleMethodArgs)!;
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

    public async Task DispatchAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request, nameof(request));

        Type requestType = typeof(TRequest);

        RequestDescriptor requestDescriptor = descriptorCache.GetDescriptor(requestType);

        object requestHandler = serviceProvider.GetRequiredService(requestDescriptor.HandlerType);

        object[] handleMethodArgs = [request, cancellationToken];

        await (Task)requestDescriptor.MethodInfo.Invoke(requestHandler, handleMethodArgs)!;
    }
}
