namespace Ocluse.LiquidSnow.Orchestrations
{
    /// <inheritdoc cref="ISkipOrchestrationResult"/>
    public readonly struct SkipOrchestrationResult : ISkipOrchestrationResult
    {
        ///<inheritdoc/>
        public readonly bool IsSuccess { get; }

        ///<inheritdoc/>
        public readonly object? Data { get; }

        ///<inheritdoc/>
        public readonly int? JumpToOrder => null;

        /// <summary>
        /// Creates a new instance of <see cref="SkipOrchestrationResult"/>.
        /// </summary>
        public SkipOrchestrationResult(object? data, bool isSuccess)
        {
            Data = data;
            IsSuccess = isSuccess;
        }
    }
}
