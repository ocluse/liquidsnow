namespace Ocluse.LiquidSnow.Http.Cqrs
{
    ///<inheritdoc cref="IKeyCommand{T}"/>
    public abstract record KeyCommand<T> : IKeyCommand<T>
    {
        ///<inheritdoc/>
        public required string Id { get; init; }
    }
}
