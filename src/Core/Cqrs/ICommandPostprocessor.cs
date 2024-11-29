using System.Reactive;

namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// Defines a handler that post processes a specific type of command after it has been executed by its main handler.
/// </summary>
public interface ICommandPostprocessor<TCommand, TCommandResult>
    where TCommand : ICommand<TCommandResult>
{
    /// <summary>
    /// Post processes the command, returning a result that overrides the original result.
    /// </summary>
    Task<TCommandResult> HandleAsync(TCommand command, TCommandResult result, CancellationToken cancellationToken = default);
}

///<inheritdoc cref="ICommandPostprocessor{TCommand, TCommandResult}"/>
public interface ICommandPostprocessor<TCommand> : ICommandPostprocessor<TCommand, Unit>
    where TCommand : ICommand
{
}
