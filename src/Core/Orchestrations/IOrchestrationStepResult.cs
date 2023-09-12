namespace Ocluse.LiquidSnow.Orchestrations
{
    /// <summary>
    /// The result of an orchestration step.
    /// </summary>
    public interface IOrchestrationStepResult
    {
        /// <summary>
        /// A value indicating whether the step was successful.
        /// </summary>
        bool IsSuccess { get; }

        /// <summary>
        /// The data that is returned by the step.
        /// </summary>
        object? Data { get; }

        /// <summary>
        /// The order of the next step to execute. If not set, the next sequential step is executed.
        /// </summary>
        int? JumpToOrder { get; }
    }

    /// <summary>
    /// A result indicating that the rest of the orchestration should be skipped.
    /// </summary>
    /// <remarks>
    /// The data returned by this result will be the final result of the orchestration.
    /// </remarks>
    public interface ISkipOrchestrationResult : IOrchestrationStepResult
    {
    }
}
