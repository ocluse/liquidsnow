namespace Ocluse.LiquidSnow.Http;

///<inheritdoc cref="IQueryRequest{TKey, Q}"/>
public record QueryRequest<TKey, Q> :  IQueryRequest<TKey, Q>
{
    ///<inheritdoc/>
    public IEnumerable<TKey>? Ids { get; set; }

    ///<inheritdoc/>
    public Q? QType { get; set; }

    ///<inheritdoc/>
    public string? Cursor { get; set; }

    ///<inheritdoc/>
    public int? Page { get; set; }

    ///<inheritdoc/>
    public int? Size { get; set; }

    ///<inheritdoc/>
    public string? Search { get; set; }
}

///<inheritdoc/>
public record QueryRequest<TKey> : QueryRequest<TKey, QueryType?>, IQueryRequest<TKey>
{

}
