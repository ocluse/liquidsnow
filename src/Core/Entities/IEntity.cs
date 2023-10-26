namespace Ocluse.LiquidSnow.Entities
{
    /// <summary>
    /// A concept or object about which information is stored
    /// </summary>
    public interface IEntity
    {
    }

    ///<inheritdoc cref="IEntity"/>
    public interface IEntity<TModel> : IEntity
    {
        /// <summary>
        /// Returns this entity represented as a model
        /// </summary>
        TModel GetModel(object? args = null);
    }
}
