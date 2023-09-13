using System;

namespace Ocluse.LiquidSnow.Entities
{
    /// <summary>
    /// An entity with a date and time the entity was created
    /// </summary>
    public interface IDatedEntity : IEntity
    {
        /// <summary>
        /// The date and time the entity was created
        /// </summary>
        DateTime CreatedAt { get; set; }
    }

    ///<inheritdoc/>
    public interface IDatedEntity<T> : IDatedEntity, IEntity<T>
        where T : IModel
    {
    }
}
