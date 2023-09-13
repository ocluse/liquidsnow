using Ocluse.LiquidSnow.Cqrs;

namespace Ocluse.LiquidSnow.Http.Cqrs
{
    /// <summary>
    /// A query that returns a list of items wrapped in a <see cref="QueryResult{T}"/>
    /// </summary>
    public interface IListQuery<T> : IQueryRequest, IQuery<QueryResult<T>>
    {

    }
}
