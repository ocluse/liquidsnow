using Ocluse.LiquidSnow.Cqrs;

namespace Ocluse.LiquidSnow.Core.Tests.Cqrs;
public record SimpleCommand : ICommand<int>
{
    public required int Value { get; set; }
}

public class SimpleCommandHandler : ICommandHandler<SimpleCommand, int>
{
    public Task<int> HandleAsync(SimpleCommand command, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(command.Value);
    }
}