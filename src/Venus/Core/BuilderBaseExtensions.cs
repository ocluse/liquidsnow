namespace Ocluse.LiquidSnow.Venus
{
    /// <summary>
    /// Extension methods for <see cref="BuilderBase"/>.
    /// </summary>
    public static class BuilderBaseExtensions
    {
        /// <summary>
        /// Adds a item to the builder.
        /// </summary>
        public static T Add<T>(this T builder, string? itemName) where T : BuilderBase
        {
            if (itemName != null)
            {
                builder.Add(itemName);
            }
            return builder;
        }

        /// <summary>
        /// Adds a item to the builder if the condition is true.
        /// </summary>
        public static T AddIf<T>(this T builder, bool condition, string? itemName) where T : BuilderBase
        {
            if (condition)
            {
                builder.Add(itemName);
            }
            return builder;
        }

        /// <summary>
        /// Adds a item to the builder if the condition is true, otherwise adds the elseClassName.
        /// </summary>
        public static T AddIfElse<T>(this T builder, bool condition, string? itemName, string? elseItemName) where T : BuilderBase
        {
            if (condition)
            {
                builder.Add(itemName);
            }
            else
            {
                builder.Add(elseItemName);
            }
            return builder;
        }

        /// <summary>
        /// Adds the item returned by a function to the builder if the condition is true.
        /// </summary>
        public static T AddIf<T>(this T builder, bool condition, Func<string?> itemName) where T : BuilderBase
        {
            if (condition)
            {
                builder.Add(itemName());
            }
            return builder;
        }

        /// <summary>
        /// Adds one or the other of items depending on a condition.
        /// </summary>
        public static T AddIfElse<T>(this T builder, bool condition, Func<string?> itemName, Func<string?> elseItemName) where T : BuilderBase
        {
            if (condition)
            {
                builder.Add(itemName());
            }
            else
            {
                builder.Add(elseItemName());
            }
            return builder;
        }

        /// <inheritdoc cref="AddIfElse{T}(T, bool, Func{string?}, Func{string?})"/>
        public static T AddIfElse<T>(this T builder, bool condition, Func<string?> itemName, string? elseItemName) where T : BuilderBase
        {
            if (condition)
            {
                builder.Add(itemName());
            }
            else
            {
                builder.Add(elseItemName);
            }
            return builder;
        }

        /// <inheritdoc cref="AddIfElse{T}(T, bool, Func{string?}, Func{string?})"/>
        public static T AddIfElse<T>(this T builder, bool condition, string? itemName, Func<string?> elseItemName) where T : BuilderBase
        {
            if (condition)
            {
                builder.Add(itemName);
            }
            else
            {
                builder.Add(elseItemName());
            }
            return builder;
        }

        /// <summary>
        /// Adds all the items to the builder.
        /// </summary>
        public static T AddAll<T>(this T builder, IEnumerable<string?> itemNames) where T : BuilderBase
        {
            foreach (var itemName in itemNames)
            {
                builder.Add(itemName);
            }
            return builder;
        }
    }
}
