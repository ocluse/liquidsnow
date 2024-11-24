using Ocluse.LiquidSnow.Orchestrations;

namespace Ocluse.LiquidSnow.Core.Tests.Orchestrations;

public record SimpleOrchestration : IOrchestration<int>
{
    public required int AddStepOne { get; init; }

    public required int AddStepTwo { get; init; }

    public const string DataKey = "Value";
}

internal class StepOne : IOrchestrationStep<SimpleOrchestration, int>
{
    public int Order => 1;

    public Task<IOrchestrationStepResult> ExecuteAsync(IOrchestrationData<SimpleOrchestration> data, CancellationToken cancellationToken = default)
    {
        var value = data.Bag.Get<int>(SimpleOrchestration.DataKey);
        value += data.Orchestration.AddStepOne;
        data.Bag.Set(SimpleOrchestration.DataKey, value);
        var result = OrchestrationStepResult.Next();
        return Task.FromResult(result);
    }
}

internal class StepTwo : IOrchestrationStep<SimpleOrchestration, int>
{
    public int Order => 2;

    public Task<IOrchestrationStepResult> ExecuteAsync(IOrchestrationData<SimpleOrchestration> data, CancellationToken cancellationToken = default)
    {
        var value = data.Bag.Get<int>(SimpleOrchestration.DataKey);
        value += data.Orchestration.AddStepTwo;
        data.Bag.Set(SimpleOrchestration.DataKey, value);
        var result = OrchestrationStepResult.Next();
        return Task.FromResult(result);
    }
}

internal class FinalStep : IOrchestrationPostprocessor<SimpleOrchestration, int>
{
    public Task<int> ExecuteAsync(IOrchestrationData<SimpleOrchestration> data, CancellationToken cancellationToken = default)
    {
        var finalResult = data.Bag.Get<int>(SimpleOrchestration.DataKey);
        return Task.FromResult(finalResult);
    }
}