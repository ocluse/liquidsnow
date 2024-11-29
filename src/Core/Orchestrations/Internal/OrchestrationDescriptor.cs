using System.Reactive;
using System.Reflection;

namespace Ocluse.LiquidSnow.Orchestrations.Internal;

internal record OrchestrationDescriptor
{
    public OrchestrationDescriptor(Type orchestrationType, Type resultType)
    {
        StepType = typeof(IOrchestrationStep<,>).MakeGenericType(orchestrationType, resultType);
        OrchestratorExecuteMethodInfo = typeof(Orchestrator).GetMethod(nameof(Orchestrator.ExecuteAsync), BindingFlags.NonPublic | BindingFlags.Instance)!
            .MakeGenericMethod(orchestrationType, resultType);
        IsUnit = resultType == typeof(Unit);
    }

    public Type StepType { get; }

    public MethodInfo OrchestratorExecuteMethodInfo { get; set; }

    public bool IsUnit { get; }

}
