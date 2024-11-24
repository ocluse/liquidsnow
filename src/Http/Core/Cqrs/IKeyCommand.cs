using Ocluse.LiquidSnow.Cqrs;

namespace Ocluse.LiquidSnow.Http.Cqrs;

/// <summary>
/// A command that has a key.
/// </summary>
public interface IKeyCommand<TKey, T> : IHasId<TKey>, ICommand<T>
{

}
