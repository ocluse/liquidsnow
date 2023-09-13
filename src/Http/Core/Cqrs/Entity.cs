using Ocluse.LiquidSnow.Entities;

namespace Ocluse.LiquidSnow.Http.Cqrs
{
    /// <summary>
    /// A base class for entities
    /// </summary>
    public abstract class Entity : IEntity
    {
        ///<inheritdoc/>
        public required string Id { get; set; }
    }

    /// <summary>
    /// A base class for entities with a model
    /// </summary>
    public abstract class Entity<T> : Entity, IEntity<T>
        where T : IModel
    {
        ///<inheritdoc/>
        public abstract T GetModel(object? args = null);
    }
}
