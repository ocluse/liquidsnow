using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Validations
{
    /// <summary>
    /// An object that is used to run multiple validations at once.
    /// </summary>
    public class ValidationSet
    {
        private readonly List<IValidatable> _items = new();

        /// <summary>
        /// Creates a new instance of <see cref="ValidationSet"/>.
        /// </summary>
        public ValidationSet(params IValidatable?[] items)
        {
            foreach (var item in items)
            {
                if (item != null)
                {
                    _items.Add(item);
                }
            }
        }

        /// <summary>
        /// Runs all validations and returns true if all validations are valid.
        /// </summary>
        public async Task<bool> Validate()
        {
            List<bool> results = new();

            foreach (var item in _items)
            {
                if (item != null)
                {
                    var result = await item.InvokeValidate();
                    results.Add(result);
                }
            }

            return !results.Any() || results.All(x => x == true);
        }

        /// <summary>
        /// Adds additional validations to the set.
        /// </summary>
        public void Add(params IValidatable?[] items)
        {
            foreach (var item in items)
            {
                if (item != null)
                {
                    _items.Add(item);
                }
            }
        }
    }
}
