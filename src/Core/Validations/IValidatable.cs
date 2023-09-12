using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Validations
{
    /// <summary>
    /// Represents an object whose state can be validated.
    /// </summary>
    public interface IValidatable
    {
        /// <summary>
        /// Runs the validation and returns true if the state of the object is valid.
        /// </summary>
        Task<bool> InvokeValidate();

        /// <summary>
        /// The most recent validation result.
        /// </summary>
        ValidationResult ValidationResult { get; set; }
    }
}
