namespace Ocluse.LiquidSnow.Venus
{
    /// <summary>
    /// Utility class for building a string of CSS classes.
    /// </summary>
    public class ClassBuilder : BuilderBase
    {
        /// <summary>
        /// Returns a well formatted css class string represented by all the classes in the builder.
        /// </summary>
        public override string Build()
        {
            return string.Join(" ", Items);
        }
    }
}
