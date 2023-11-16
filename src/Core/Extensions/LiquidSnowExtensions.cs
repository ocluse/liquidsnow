using Ocluse.LiquidSnow.Entities;

namespace Ocluse.LiquidSnow.Extensions
{
    /// <summary>
    /// Adds extension methods to the LiquidSnow library
    /// </summary>
    public static class LiquidSnowExtensions
    {
        /// <summary>
        /// Returns the models of the provided entities
        /// </summary>
        public static IEnumerable<TModel> Models<TModel>(this IEnumerable<IEntity<TModel>> entities)
        {
            if(entities == null)
            {
                throw new System.ArgumentNullException(nameof(entities));
            }
            return entities.Select(x => x.GetModel());
        }
    }
}
