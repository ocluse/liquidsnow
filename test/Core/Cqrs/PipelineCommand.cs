using Ocluse.LiquidSnow.Cqrs;

namespace Ocluse.LiquidSnow.Core.Tests.Cqrs;

internal record PipelineCommand : ICommand<int>
{
    public required int InitialValue { get; init; }

    public required int HandleAddition { get; init; }

    public required int PreprocessAddition { get; init; }

    public required int PostprocessAddition { get; init; }

    public int Value { get; set; }
}

internal class PipelineCommandPreprocessor : ICommandPreprocessor<PipelineCommand, int>
{
    public Task<PipelineCommand> HandleAsync(PipelineCommand command, CancellationToken cancellationToken = default)
    {
        command.Value = command.InitialValue + command.PreprocessAddition;

        return Task.FromResult(command);
    }
}

internal class PipelineCommandHandler : ICommandHandler<PipelineCommand, int>
{
    public Task<int> HandleAsync(PipelineCommand command, CancellationToken cancellationToken = default)
    {
        command.Value += command.HandleAddition;

        return Task.FromResult(command.Value);
    }
}

internal class PipelineCommandPostprocessor : ICommandPostprocessor<PipelineCommand, int>
{
    public Task<int> HandleAsync(PipelineCommand command, int result, CancellationToken cancellationToken = default)
    {
        var finalResult = result + command.PostprocessAddition;
        return Task.FromResult(finalResult);
    }
}