namespace Ocluse.LiquidSnow.Orchestrations.Internal;

internal class OrchestrationData<T>(T value) : IOrchestrationData<T>
{
    private readonly List<IOrchestrationStepResult> _results = [];

    public IReadOnlyList<IOrchestrationStepResult> Results => _results;

    public IOrchestrationBag Bag { get; } = new OrchestrationBag();

    public T Orchestration { get; } = value;

    public void AddResult(IOrchestrationStepResult result)
    {
        _results.Add(result);
    }
}
