using Ocluse.LiquidSnow.Cqrs;

namespace Ocluse.LiquidSnow.Http.Cqrs
{
    ///<inheritdoc cref="IListQuery{T}"/>
    public abstract record ListQuery<T> : QueryRequest, IListQuery<T>
    {
        
    }
}
