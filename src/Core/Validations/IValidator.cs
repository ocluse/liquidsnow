using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Validations
{
    /// <summary>
    /// Represents an entity that can validate a value.
    /// </summary>
    /// <typeparam name="T">The type of the value being validated</typeparam>
    public interface IValidator<T>
    {
        /// <summary>
        /// Validates a value and returns the result.
        /// </summary>
        Task<ValidationResult> Validate([MaybeNull] T value);
    }
}