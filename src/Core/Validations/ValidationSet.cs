namespace Ocluse.LiquidSnow.Validations
{
    /// <summary>
    /// An object that is used to run multiple validations at once.
    /// </summary>
    public class ValidationSet
    {
        private readonly List<IValidatable> _items = [];

        /// <summary>
        /// Creates a new instance of <see cref="ValidationSet"/>.
        /// </summary>
        public ValidationSet(params IValidatable?[] items)
        {
            foreach (var item in items)
            {
                Add(item);
            }
        }

        /// <summary>
        /// Runs all validations and returns true if all validations are valid.
        /// </summary>
        public async Task<bool> Validate()
        {
            bool result = true;

            foreach (var item in _items)
            {
                var isValid = await item.InvokeValidate();

                if (!isValid)
                {
                    result = false;
                }
            }

            return result;
        }

        /// <summary>
        /// Adds as a validation to the set.
        /// </summary>
        public ValidationSet Add(IValidatable? item)
        {
            if(item != null)
            {
                _items.Add(item);
            }

            return this;
        }

        /// <summary>
        /// Adds additional validations to the set.
        /// </summary>
        public ValidationSet Add(params IValidatable?[] items)
        {
            return AddRange(items);
        }

        /// <summary>
        /// Adds additional validations to the set if the condition is true.
        /// </summary>
        public ValidationSet AddIf(bool condition, params IValidatable?[] items)
        {
            if (condition)
            {
                return AddRange(items);
            }
            return this;
        }

        /// <summary>
        /// Adds a validation to the set if the condition is true, otherwise adds the else validation.
        /// </summary>
        public ValidationSet AddIfElse(bool condition, IValidatable item, IValidatable elseItem)
        {
            if (condition)
            {
                return Add(item);
            }
            else
            {
                return Add(elseItem);
            }
        }

        /// <summary>
        /// Adds a range of validations to the set.
        /// </summary>
        public ValidationSet AddRange(IEnumerable<IValidatable?> items)
        {
            foreach (var item in items)
            {
                Add(item);
            }

            return this;
        }

        /// <summary>
        /// Removes all validations from the set.
        /// </summary>
        public ValidationSet Clear()
        {
            _items.Clear();
            return this;
        }

        /// <summary>
        /// Executes the provided action if all validations in the set are valid.
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public async Task Run(Action action)
        {
            if (await Validate())
            {
                action();
            }
        }

        /// <summary>
        /// Executes the provided function if all validations in the set are valid and returns the result.
        /// Otherwise it returns the default value of the type.
        /// </summary>
        public async Task<T?> Run<T>(Func<T> func)
        {
            if (await Validate())
            {
                return func();
            }
            return default;
        }

        /// <summary>
        /// Executes the provided function if all validations in the set are valid.
        /// </summary>
        public async Task<bool> RunAsync(Func<Task> action)
        {
            if (await Validate())
            {
                await action();
                return true;
            }
            return false;
        }

        /// <summary>
        /// Executes the provided function asynchronously if all validations in the set are valid and returns the result.
        /// Otherwise it returns the default value of the type.
        /// </summary>
        public async Task<T?> RunAsync<T>(Func<Task<T?>> action)
        {
            if (await Validate())
            {
                return await action();
            }
            return default;
        }
    }
}
