namespace Ocluse.LiquidSnow.Validations
{
    /// <summary>
    /// Provides a set of common validators.
    /// </summary>
    public static class Validators
    {
        /// <summary>
        /// Provides a set of common validators.
        /// </summary>
        public static CommonValidators Common { get; } = new CommonValidators();

        /// <summary>
        /// Checks if all the items are valid.
        /// </summary>
        public static Task<bool> Check(params IValidatable?[] items)
        {
            ValidationSet set = new(items);
            return set.Validate();
        }

        /// <summary>
        /// Creates a new <see cref="ValidationSet"/> with the provided items.
        /// </summary>
        public static ValidationSet With(params IValidatable?[] items)
        {
            return new ValidationSet(items);
        }

        /// <summary>
        /// Creates a new <see cref="ValidationSet"/> with the provided item.
        /// </summary>
        public static ValidationSet With(this IValidatable? item)
        {
            return new ValidationSet(item);
        }

        /// <summary>
        /// Creates a new <see cref="ValidationSet"/> with the provided item if the condition is true.
        /// </summary>
        public static ValidationSet WithIf(this IValidatable? item, bool condition)
        {
            return condition ? new ValidationSet(item) : new ValidationSet();
        }
    }
}
