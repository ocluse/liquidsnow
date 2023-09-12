using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Validations
{
    /// <summary>
    /// Common validators useful for validating common types.
    /// </summary>
    public class CommonValidators
    {
        /// <summary>
        /// The message to display when a value is required but not provided.
        /// </summary>
        public string ValueRequiredMessage { get; set; } = "This field is required";

        /// <summary>
        /// The message to display when a value must be greater than zero but is not.
        /// </summary>
        public string ValueMustBeGreaterThanZeroMessage { get; set; } = "This field must be greater than zero";

        /// <summary>
        /// The message to display when a value is not in a valid format.
        /// </summary>
        public string FormatInvalidMessage { get; set; } = "This field is not in a valid format";

        /// <summary>
        /// Validates that the provided value is not null.
        /// </summary>
        public Task<ValidationResult> NotNull<T>([MaybeNull]T item)
        {
            if (item == null)
            {
                return Task.FromResult(ValidationResult.Failed(ValueRequiredMessage));
            }
            return Task.FromResult(ValidationResult.Succeeded());
        }

        /// <summary>
        /// Validates that the provided string is not null or empty.
        /// </summary>
        public Task<ValidationResult> StringNotNull(string? value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return Task.FromResult(ValidationResult.Failed(ValueRequiredMessage));
            }

            return Task.FromResult(ValidationResult.Succeeded());
        }

        /// <summary>
        /// Validates that the provided int is not null
        /// </summary>
        public Task<ValidationResult> IntNotNull(int? value)
        {
            if (value == null)
            {
                return Task.FromResult(ValidationResult.Failed(ValueRequiredMessage));
            }
            return Task.FromResult(ValidationResult.Succeeded());
        }

        /// <summary>
        /// Validates that the provided integer value is greater than zero.
        /// </summary>
        public Task<ValidationResult> IntGreaterThanZero(int value)
        {
            if (value <= 0)
            {
                return Task.FromResult(ValidationResult.Failed(ValueMustBeGreaterThanZeroMessage));
            }
            return Task.FromResult(ValidationResult.Succeeded());
        }

        /// <summary>
        /// Validates that the provided decimal value is greater than zero.
        /// </summary>
        public Task<ValidationResult> DecimalGreaterThanZero(decimal value)
        {
            if (value <= 0)
            {
                return Task.FromResult(ValidationResult.Failed("This field must be greater than zero"));
            }
            return Task.FromResult(ValidationResult.Succeeded());
        }

        /// <summary>
        /// Validates that the provided string is a valid Uri
        /// </summary>
        public Task<ValidationResult> IsUri(string? value)
        {
            if (value == null)
            {
                return Task.FromResult(ValidationResult.Failed(ValueRequiredMessage));
            }
            if (!Uri.TryCreate(value, UriKind.Absolute, out Uri? uriResult))
            {
                return Task.FromResult(ValidationResult.Failed(FormatInvalidMessage));
            }
            if (uriResult.Scheme != Uri.UriSchemeHttp && uriResult.Scheme != Uri.UriSchemeHttps)
            {
                return Task.FromResult(ValidationResult.Failed(FormatInvalidMessage));
            }
            return Task.FromResult(ValidationResult.Succeeded());
        }

        /// <summary>
        /// Validates that the value is not null and is a valid DateTime
        /// </summary>
        public Task<ValidationResult> DateTimeNotNull(DateTime? value)
        {
            if (value == null)
            {
                return Task.FromResult(ValidationResult.Failed(ValueRequiredMessage));
            }
            return Task.FromResult(ValidationResult.Succeeded());
        }
    }
}
