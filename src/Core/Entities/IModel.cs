namespace Ocluse.LiquidSnow.Entities
{
    /// <summary>
    /// A selective representation of an <see cref="IEntity"/> whose form and content are chosen based on a specific set of concerns
    /// </summary>
    public interface IModel
    {
        /// <summary>
        /// THe unique identifier of the model
        /// </summary>
        string Id { get; set; }
    }
}
