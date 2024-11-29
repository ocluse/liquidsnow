namespace Ocluse.LiquidSnow.Requests;

/// <summary>
/// Defines a handler that processes a specific type of request.
/// </summary>
public interface IRequestHandler<T, TResult>
{
    /// <summary>
    /// Handles the request, returning the result.
    /// </summary>
    Task<TResult> HandleAsync(T request, CancellationToken cancellationToken = default);
}

///<inheritdoc cref="IRequestHandler{T, TResult}"/>
public interface IRequestHandler<T>
{
    /// <summary>
    /// Handles the request.
    /// </summary>
    Task HandleAsync(T request, CancellationToken cancellationToken = default);
}