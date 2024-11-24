using Ocluse.LiquidSnow.Orchestrations;

namespace Ocluse.LiquidSnow.Core.Tests.Orchestrations;

public class ConditionalOrchestration : IOrchestration<int>
{
    public required int AddPreprocessStep { get; init; }

    public required int AddPostprocessStep { get; init; }

    public required int AddInJumpedStep { get; init; }

    public required bool ProduceGoToResultInPreprocess { get; init; }

    public required bool ExecuteFirstCondition { get; init; }

    public required bool ExecuteSecondCondition { get; init; }

    public required int AddIfFirstCondition { get; init; }

    public required int AddIfSecondCondition { get; init; }

    public int Value { get; set; }
}

internal class PreprocessStep : IOrchestrationPreprocessor<ConditionalOrchestration, int>
{
    public Task<IOrchestrationStepResult> ExecuteAsync(IOrchestrationData<ConditionalOrchestration> data, CancellationToken cancellationToken = default)
    {
        data.Orchestration.Value = data.Orchestration.AddPreprocessStep;
        var result = data.Orchestration.ProduceGoToResultInPreprocess ? OrchestrationStepResult.GoTo(10) : OrchestrationStepResult.Next();
        return Task.FromResult(result);
    }
}

internal class JumpedStep : IOrchestrationStep<ConditionalOrchestration, int>
{
    public int Order => 0;

    public Task<IOrchestrationStepResult> ExecuteAsync(IOrchestrationData<ConditionalOrchestration> data, CancellationToken cancellationToken = default)
    {
        data.Orchestration.Value += data.Orchestration.AddInJumpedStep;
        return Task.FromResult(OrchestrationStepResult.Next());
    }
}

internal class FirstConditionStep : IConditionalOrchestrationStep<ConditionalOrchestration, int>
{
    public int Order => 10;

    public Task<bool> CanExecute(IOrchestrationData<ConditionalOrchestration> data, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(data.Orchestration.ExecuteFirstCondition);
    }

    public Task<IOrchestrationStepResult> ExecuteAsync(IOrchestrationData<ConditionalOrchestration> data, CancellationToken cancellationToken = default)
    {
        data.Orchestration.Value += data.Orchestration.AddIfFirstCondition;
        return Task.FromResult(OrchestrationStepResult.Next());
    }
}

internal class SecondConditionStep : IConditionalOrchestrationStep<ConditionalOrchestration, int>
{
    public int Order => 20;

    public Task<bool> CanExecute(IOrchestrationData<ConditionalOrchestration> data, CancellationToken cancellationToken = default)
    {
        return Task.FromResult(data.Orchestration.ExecuteSecondCondition);
    }

    public Task<IOrchestrationStepResult> ExecuteAsync(IOrchestrationData<ConditionalOrchestration> data, CancellationToken cancellationToken = default)
    {
        data.Orchestration.Value += data.Orchestration.AddIfSecondCondition;
        return Task.FromResult(OrchestrationStepResult.Next());
    }
}

internal class PostprocessStep : IOrchestrationPostprocessor<ConditionalOrchestration, int>
{
    public Task<int> ExecuteAsync(IOrchestrationData<ConditionalOrchestration> data, CancellationToken cancellationToken = default)
    {
        data.Orchestration.Value += data.Orchestration.AddPostprocessStep;
        return Task.FromResult(data.Orchestration.Value);
    }
}