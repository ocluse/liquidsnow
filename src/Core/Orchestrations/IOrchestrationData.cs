using System.Collections.Generic;

namespace Ocluse.LiquidSnow.Orchestrations
{
    /// <summary>
    /// The data that is passed between steps in an orchestration.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public interface IOrchestrationData<out T>
    {
        /// <summary>
        /// The results of each step in the orchestration.
        /// </summary>
        IReadOnlyList<IOrchestrationStepResult> Results { get; }

        /// <summary>
        /// The bag of data that is shared between steps in the orchestration.
        /// </summary>
        IOrchestrationBag Bag { get; }

        /// <summary>
        /// The orchestration that is being executed.
        /// </summary>
        T Orchestration { get; }

    }
}
