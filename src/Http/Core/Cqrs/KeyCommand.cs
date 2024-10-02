namespace Ocluse.LiquidSnow.Http.Cqrs;

///<inheritdoc cref="IKeyCommand{TKey, T}"/>
public abstract record KeyCommand<TKey, T> : IKeyCommand<TKey, T>
{
    ///<inheritdoc/>
    public required TKey Id { get; init; }
}
