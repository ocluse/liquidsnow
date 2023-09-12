using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Venus
{
    /// <summary>
    /// Utility class for building a string of CSS classes.
    /// </summary>
    public class ClassBuilder
    {
        private readonly List<string> _classes = new();

        /// <summary>
        /// Adds a class to the builder.
        /// </summary>
        public ClassBuilder Add(string? className)
        {
            if (className != null)
            {
                _classes.Add(className);
            }
            return this;
        }

        /// <summary>
        /// Adds a class to the builder if the condition is true.
        /// </summary>
        public ClassBuilder AddIf(bool condition, string? className)
        {
            if (condition)
            {
                Add(className);
            }
            return this;
        }

        /// <summary>
        /// Adds a class to the builder if the condition is true, otherwise adds the elseClassName.
        /// </summary>
        public ClassBuilder AddIfElse(bool condition, string? className, string? elseClassName)
        {
            if (condition)
            {
                Add(className);
            }
            else
            {
                Add(elseClassName);
            }
            return this;
        }

        /// <summary>
        /// Adds the class returned by a function to the builder if the condition is true.
        /// </summary>
        public ClassBuilder AddIf(bool condition, Func<string?> className)
        {
            if (condition)
            {
                Add(className());
            }
            return this;
        }

        /// <summary>
        /// Adds one or the other of classes depending on a condition.
        /// </summary>
        public ClassBuilder AddIfElse(bool condition, Func<string?> className, Func<string?> elseClassName)
        {
            if (condition)
            {
                Add(className());
            }
            else
            {
                Add(elseClassName());
            }
            return this;
        }

        /// <inheritdoc cref="AddIfElse(bool, Func{string?}, Func{string?})"/>
        public ClassBuilder AddIfElse(bool condition, Func<string?> className, string? elseClassName)
        {
            if (condition)
            {
                Add(className());
            }
            else
            {
                Add(elseClassName);
            }
            return this;
        }

        /// <inheritdoc cref="AddIfElse(bool, Func{string?}, Func{string?})"/>
        public ClassBuilder AddIfElse(bool condition, string? className, Func<string?> elseClassName)
        {
            if (condition)
            {
                Add(className);
            }
            else
            {
                Add(elseClassName());
            }
            return this;
        }

        /// <summary>
        /// Adds all the classes to the builder.
        /// </summary>
        /// <param name="classNames"></param>
        /// <returns></returns>
        public ClassBuilder AddAll(IEnumerable<string?> classNames)
        {
            foreach (var className in classNames)
            {
                Add(className);
            }
            return this;
        }

        /// <summary>
        /// Returns a well formatted css class string represented by all the classes in the builder.
        /// </summary>
        public string Build()
        {
            return string.Join(" ", _classes);
        }
    }
}
