namespace Ocluse.LiquidSnow.Entities
{
    /// <summary>
    /// A concept or object that represents an entity whose equality is not based on identity.
    /// </summary>
    public abstract class ValueObject
    {
        /// <summary>
        /// Check if the two objects are equal based on their equality components.
        /// </summary>
        protected static bool EqualOperator(ValueObject left, ValueObject right)
        {
            if (left is null ^ right is null)
            {
                return false;
            }

            return left?.Equals(right!) != false;
        }

        /// <summary>
        /// Check if two objects are not equal based on their equality components.
        /// </summary>
        protected static bool NotEqualOperator(ValueObject left, ValueObject right)
        {
            return !EqualOperator(left, right);
        }

        /// <summary>
        /// Returns the properties of the object used to determine whether it is the same as another
        /// </summary>
        protected abstract IEnumerable<object?> GetEqualityComponents();

        ///<inheritdoc/>
        public override bool Equals(object? obj)
        {
            if (obj == null || obj.GetType() != GetType())
            {
                return false;
            }

            var other = (ValueObject)obj;
            return GetEqualityComponents().SequenceEqual(other.GetEqualityComponents());
        }

        /// <summary>
        /// Gets the hash code of the object based on its equality components.
        /// </summary>
        public override int GetHashCode()
        {
            return GetEqualityComponents()
                .Select(x => x != null ? x.GetHashCode() : 0)
                .Aggregate((x, y) => x ^ y);
        }
    }
}
