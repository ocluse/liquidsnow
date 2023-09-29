using System.Text;

namespace Ocluse.LiquidSnow.Venus
{
    /// <summary>
    /// Utility class for building a string of CSS styles.
    /// </summary>
    public class StyleBuilder : BuilderBase
    {
        /// <summary>
        /// Returns a well formatted css style string represented by all the styles in the builder.
        /// </summary>
        public override string Build()
        {
            if (Items.Count == 0)
            {
                return string.Empty;
            }

            StringBuilder sb = new();

            foreach (var item in Items)
            {
                sb.Append(item);

                if (!item.EndsWith(';'))
                {
                    sb.Append(';');
                }
            }

            return sb.ToString();
        }
    }
}
