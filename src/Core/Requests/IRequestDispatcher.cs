namespace Ocluse.LiquidSnow.Requests;

/// <summary>
/// Defines mechanisms for routing requests to their appropriate handler.
/// </summary>
public interface IRequestDispatcher
{
    /// <summary>
    /// Dispatches a request to its appropriate handler and returns the result.
    /// </summary>
    Task<TResult> DispatchAsync<TResult>(object request, CancellationToken cancellationToken = default);

    ///<see cref="DispatchAsync{TResult}(object, CancellationToken)"/>
    Task<TResult> DispatchAsync<TRequest, TResult>(TRequest request, CancellationToken cancellationToken = default);

    /// <summary>
    /// Dispatches a request to its appropriate handler.
    /// </summary>
    Task DispatchAsync<TRequest>(TRequest request, CancellationToken cancellationToken = default);
}
