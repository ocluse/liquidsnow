namespace Ocluse.LiquidSnow.Validations
{
    /// <summary>
    /// Represents the result of a validation.
    /// </summary>
    /// <param name="Message">A message describing the result of the validation.</param>
    /// <param name="IsValid">A value indicating whether the state of the object is valid.</param>
    public record ValidationResult(bool IsValid, string? Message)
    {
        /// <summary>
        /// A <see cref="ValidationResult"/> describing a valid state.
        /// </summary>
        public static ValidationResult ValidResult { get; } = new ValidationResult(true, null);

        /// <summary>
        /// Implicitly converts a <see cref="bool"/> to a <see cref="ValidationResult"/>.
        /// </summary>
        public static implicit operator ValidationResult(bool success)
        {
            return new ValidationResult(success, null);
        }

        /// <summary>
        /// Implicitly converts a <see cref="ValidationResult"/> to a <see cref="bool"/>.
        /// </summary>
        public static implicit operator bool(ValidationResult result)
        {
            return result.IsValid;
        }

        /// <summary>
        /// Creates a new instance of <see cref="ValidationResult"/> indicating that the state of the object is invalid.
        /// </summary>
        public static ValidationResult NotValid(string? message)
        {
            return new ValidationResult(false, message);
        }
    }
}
