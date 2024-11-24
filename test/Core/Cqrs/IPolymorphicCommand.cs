using Ocluse.LiquidSnow.Cqrs;

namespace Ocluse.LiquidSnow.Core.Tests.Cqrs;

public record PolymorphicCommand : ICommand<int>
{
    public int Value { get; init; }
}

public record AdditivePolymorphicCommand : PolymorphicCommand
{
    public int Added { get; init; }
}

public record SubtractivePolymorphicCommand : PolymorphicCommand
{
    public int Subtracted { get; init; }
}

public class PolymorphicCommandHandler : ICommandHandler<PolymorphicCommand, int>
{
    public Task<int> HandleAsync(PolymorphicCommand command, CancellationToken cancellationToken = default)
    {
        int result = command.Value;

        if(command is AdditivePolymorphicCommand additiveCommand)
        {
            result += additiveCommand.Added;
        }
        else if (command is SubtractivePolymorphicCommand subtractiveCommand)
        {
            result -= subtractiveCommand.Subtracted;
        }
        else
        {
            throw new InvalidOperationException("Unknown command type.");
        }

        return Task.FromResult(result);
    }
}