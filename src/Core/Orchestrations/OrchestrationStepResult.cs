using System.Reactive;

namespace Ocluse.LiquidSnow.Orchestrations
{

    ///<inheritdoc cref="IOrchestrationStepResult"/>
    public readonly struct OrchestrationStepResult : IOrchestrationStepResult
    {
        ///<inheritdoc cref="IOrchestrationStepResult.IsSuccess"/>
        public bool IsSuccess { get; }

        ///<inheritdoc cref="IOrchestrationStepResult.Data"/>
        public object? Data { get; }

        ///<inheritdoc cref="IOrchestrationStepResult.JumpToOrder"/>
        public int? JumpToOrder { get; }

        /// <summary>
        /// Creates a new instance of <see cref="OrchestrationStepResult"/>.
        /// </summary>
        public OrchestrationStepResult(bool isSuccess, object? data, int? jumpToStep)
        {
            IsSuccess = isSuccess;
            Data = data;
            JumpToOrder = jumpToStep;
        }

        /// <summary>
        /// Returns a result indicating that the step was successfully executed.
        /// </summary>
        public static IOrchestrationStepResult Success(object? data = null, int? jumpToStep = null)
        {
            return new OrchestrationStepResult(true, data, jumpToStep);
        }

        /// <summary>
        /// Returns a result indicating that the step failed.
        /// </summary>
        public static IOrchestrationStepResult Failed(object? data = null, int? jumpToStep = null)
        {
            return new OrchestrationStepResult(false, data, jumpToStep);
        }

        /// <summary>
        /// Skips the entire orchestration.
        /// </summary>
        public static ISkipOrchestrationResult Skip(object? data = null, bool isSuccess = true)
        {
            return new SkipOrchestrationResult(data, isSuccess);
        }

        /// <summary>
        /// Skips the entire orchestration and returns <see cref="Unit"/> as the result.
        /// </summary>
        /// <remarks>
        /// This is a convenience method for when the orchestration is not returning any data, 
        /// and it is typically used with the non-generic <see cref="IOrchestration"/> interface.
        /// </remarks>
        public static ISkipOrchestrationResult SkipAsUnit(bool isSuccess = true)
        {
            return new SkipOrchestrationResult(default, isSuccess);
        }
    }
}
