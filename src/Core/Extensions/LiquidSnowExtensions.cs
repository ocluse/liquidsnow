using Ocluse.LiquidSnow.Entities;
using System.Collections.Generic;
using System.Linq;

namespace Ocluse.LiquidSnow.Extensions
{
    /// <summary>
    /// Adds extension methods to the LiquidSnow library
    /// </summary>
    public static class LiquidSnowExtensions
    {
        /// <summary>
        /// Returns a list of entities as their model equivalents
        /// </summary>
        /// <remarks>
        /// If the source is null, an empty list is returned
        /// </remarks>
        public static IReadOnlyList<TModel> GetModels<TModel>(this IEnumerable<IEntity<TModel>> entities)
            where TModel : IModel
        {
            return entities?.Select(x => x.GetModel()).ToList() ?? new List<TModel>();
        }
    }
}
