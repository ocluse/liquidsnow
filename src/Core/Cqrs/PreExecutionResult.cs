namespace Ocluse.LiquidSnow.Cqrs
{
    /// <summary>
    /// The result of a pre-execution handler.
    /// </summary>
    public abstract class PreExecutionResult
    {
        /// <summary>
        /// Creates a result that indicates the command should be executed.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="ContinuePreExecutionResult"/> which indicates the command should be executed.
        /// </returns>
        public static ContinuePreExecutionResult Continue() => new();

        /// <summary>
        /// Creates a result that indicates the command should not be executed, and the provided value should be used as the result.
        /// </summary>
        /// <returns>
        /// Returns a <see cref="StopPreExecutionResult{T}"/> which indicates the command should not be executed, and the provided value should be used as the result.
        /// </returns>
        public static StopPreExecutionResult<T> Stop<T>(T value) => new(value);

        /// <summary>
        /// A result that indicates the command should be executed.
        /// </summary>
        public sealed class ContinuePreExecutionResult : PreExecutionResult
        {

        }

        /// <summary>
        /// A result that indicates the command should not be executed, and the provided value should be used as the result.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        public sealed class StopPreExecutionResult<T> : PreExecutionResult
        {

            /// <summary>
            /// Creates a result that indicates the command should not be executed, and the provided value should be used as the result.
            /// </summary>
            /// <param name="value">The value to use as the result</param>
            public StopPreExecutionResult(T value)
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
            public static implicit operator T(StopPreExecutionResult<T> result) => result.Value;
        }
    }


}
