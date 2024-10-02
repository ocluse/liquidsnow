namespace Ocluse.LiquidSnow.Cqrs;

/// <summary>
/// The result of a pre-execution handler.
/// </summary>
public abstract class PreExecutionResult
{
    /// <summary>
    /// Creates a result that indicates the command/query should continue into execution.
    /// </summary>
    public static ContinueExecutionResult Continue() => new();

    /// <summary>
    /// Creates a result that indicates the command/query should not continue into execution.
    /// </summary>
    public static HaltExecutionResult<T> Halt<T>(T value) => new(value);

    /// <summary>
    /// Creates an instance of the <see cref="ContinueExecutionResult"/> class.
    /// </summary>
    public sealed class ContinueExecutionResult : PreExecutionResult
    {

    }

    /// <summary>
    /// A result that indicates the command should not continue into execution, and the provided value should be used as the result.
    /// </summary>
    public sealed class HaltExecutionResult<T> : PreExecutionResult
    {

        /// <summary>
        /// Creates an instance of the <see cref="HaltExecutionResult{T}"/> class.
        /// </summary>
        /// <param name="value"></param>
        internal HaltExecutionResult(T value)
        {
            Value = value;
        }

        /// <summary>
        /// The value to use as the result.
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// A conversion operator that allows the result to be used as the value.
        /// </summary>
        /// <param name="result"></param>
        public static implicit operator T(HaltExecutionResult<T> result) => result.Value;
    }
}
