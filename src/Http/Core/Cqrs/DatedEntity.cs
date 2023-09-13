using Ocluse.LiquidSnow.Entities;

namespace Ocluse.LiquidSnow.Http.Cqrs
{
    ///<inheritdoc cref="IDatedEntity"/>
    public abstract class DatedEntity : Entity, IDatedEntity
    {
        /// <summary>
        /// The date and time the entity was created
        /// </summary>
        public required DateTime CreatedAt { get; set; }
    }

    ///<inheritdoc/>
    public abstract class DatedEntity<T> : DatedEntity, IDatedEntity<T>
        where T : IModel
    {
        ///<inheritdoc/>
        public abstract T GetModel(object? args = null);
    }
}
