namespace Ocluse.LiquidSnow.Orchestrations
{
    /// <summary>
    /// Represents the required state for the execution of an 
    /// <see cref="IStateDependentOrchestrationStep{T, TResult}"/>
    /// </summary>
    public enum RequiredState
    {
        /// <summary>
        /// The execution will only be attempted if the previous step was successful.
        /// </summary>
        Success,

        /// <summary>
        /// The execution will only be attempted if the previous step failed.
        /// </summary>
        Failure
    }
}
