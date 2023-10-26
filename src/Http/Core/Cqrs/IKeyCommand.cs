using Ocluse.LiquidSnow.Cqrs;

namespace Ocluse.LiquidSnow.Http.Cqrs
{
    /// <summary>
    /// A command that has a key.
    /// </summary>
    public interface IKeyCommand<TKey, T> : ICommand<T>
    {
        /// <summary>
        /// The key of the command.
        /// </summary>
        TKey Id { get; }

    }
}
