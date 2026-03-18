using Microsoft.Extensions.DependencyInjection;

namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Provides strongly typed CQRS execution helpers used by generated dispatch code.
/// </summary>
public static class CqrsDispatchExecutor
{
    /// <summary>
    /// Executes a command pipeline.
    /// </summary>
    public static async Task<TCommandResult> ExecuteCommandAsync<TCommand, TCommandResult>(
        TCommand command,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TCommandResult>
    {
        ICommandPreprocessor<TCommand, TCommandResult>? preprocessor =
            serviceProvider.GetService<ICommandPreprocessor<TCommand, TCommandResult>>();

        if (preprocessor != null)
        {
            command = await preprocessor.HandleAsync(command, cancellationToken);
        }

        ICommandHandler<TCommand, TCommandResult> handler =
            serviceProvider.GetRequiredService<ICommandHandler<TCommand, TCommandResult>>();

        TCommandResult result = await handler.HandleAsync(command, cancellationToken);

        ICommandPostprocessor<TCommand, TCommandResult>? postprocessor =
            serviceProvider.GetService<ICommandPostprocessor<TCommand, TCommandResult>>();

        if (postprocessor != null)
        {
            result = await postprocessor.HandleAsync(command, result, cancellationToken);
        }

        return result;
    }

    /// <summary>
    /// Executes a command pipeline with a boxed request value.
    /// </summary>
    public static async Task<object> ExecuteCommandBoxedAsync<TCommand, TCommandResult>(
        object command,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
        where TCommand : ICommand<TCommandResult>
    {
        TCommand typedCommand = (TCommand)command;
        TCommandResult result = await ExecuteCommandAsync<TCommand, TCommandResult>(typedCommand, serviceProvider, cancellationToken);
        return result!;
    }

    /// <summary>
    /// Executes a query pipeline.
    /// </summary>
    public static async Task<TQueryResult> ExecuteQueryAsync<TQuery, TQueryResult>(
        TQuery query,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TQueryResult>
    {
        IQueryPreprocessor<TQuery, TQueryResult>? preprocessor =
            serviceProvider.GetService<IQueryPreprocessor<TQuery, TQueryResult>>();

        if (preprocessor != null)
        {
            query = await preprocessor.HandleAsync(query, cancellationToken);
        }

        IQueryHandler<TQuery, TQueryResult> handler =
            serviceProvider.GetRequiredService<IQueryHandler<TQuery, TQueryResult>>();

        TQueryResult result = await handler.HandleAsync(query, cancellationToken);

        IQueryPostprocessor<TQuery, TQueryResult>? postprocessor =
            serviceProvider.GetService<IQueryPostprocessor<TQuery, TQueryResult>>();

        if (postprocessor != null)
        {
            result = await postprocessor.HandleAsync(query, result, cancellationToken);
        }

        return result;
    }

    /// <summary>
    /// Executes a query pipeline with a boxed request value.
    /// </summary>
    public static async Task<object> ExecuteQueryBoxedAsync<TQuery, TQueryResult>(
        object query,
        IServiceProvider serviceProvider,
        CancellationToken cancellationToken = default)
        where TQuery : IQuery<TQueryResult>
    {
        TQuery typedQuery = (TQuery)query;
        TQueryResult result = await ExecuteQueryAsync<TQuery, TQueryResult>(typedQuery, serviceProvider, cancellationToken);
        return result!;
    }
}
