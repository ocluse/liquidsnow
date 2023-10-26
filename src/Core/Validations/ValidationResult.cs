namespace Ocluse.LiquidSnow.Validations
{
    /// <summary>
    /// Represents the result of a validation.
    /// </summary>
    public readonly struct ValidationResult
    {
        /// <summary>
        /// Creates a new instance of <see cref="ValidationResult"/> indicating whether the validation was successful and a message describing the result.
        /// </summary>
        public ValidationResult(bool success, string? message)
        {
            Success = success;
            Message = message;
        }

        /// <summary>
        /// A boolean indicating whether the state of the object is valid.
        /// </summary>
        /// <param name="success"></param>
        public static implicit operator ValidationResult(bool success)
        {
            return new ValidationResult(success, null);
        }


        /// <summary>
        /// A message describing the result of the validation.
        /// </summary>
        public string? Message { get; }

        /// <summary>
        /// A boolean indicating whether the state of the object is valid.
        /// </summary>
        public bool Success { get; }

        /// <summary>
        /// Creates a new instance of <see cref="ValidationResult"/> indicating that the validation succeeded.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="ValidationResult"/> with <see cref="Success"/> set to true and <see cref="Message"/> set to null.
        /// </returns>
        public static ValidationResult Succeeded()
        {
            return new ValidationResult(true, null);
        }

        /// <summary>
        /// Creates a new instance of <see cref="ValidationResult"/> indicating that the validation failed.
        /// </summary>
        /// <returns>
        /// An instance of <see cref="ValidationResult"/> with <see cref="Success"/> set to false and <see cref="Message"/> set to the specified message.
        /// </returns>
        public static ValidationResult Failed(string? message)
        {
            return new ValidationResult(false, message);
        }
    }
}
