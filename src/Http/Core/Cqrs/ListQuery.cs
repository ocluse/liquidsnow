namespace Ocluse.LiquidSnow.Http.Cqrs;

///<inheritdoc cref="IListQuery{TKey, T}"/>
public abstract record ListQuery<TKey, T> : QueryRequest<TKey>, IListQuery<TKey, T>
{
    
}
