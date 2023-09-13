using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Http
{
    ///<inheritdoc cref="IQueryRequest{Q}"/>
    public record QueryRequest<Q> :  IQueryRequest<Q>
    {
        ///<inheritdoc/>
        public IEnumerable<string>? Ids { get; init; }

        ///<inheritdoc/>
        public Q? QType { get; set; }

        ///<inheritdoc/>
        public string? Cursor { get; init; }

        ///<inheritdoc/>
        public int? Page { get; init; }

        ///<inheritdoc/>
        public int? Size { get; init; }

        ///<inheritdoc/>
        public string? Search { get; init; }
    }

    ///<inheritdoc/>
    public record QueryRequest : QueryRequest<QueryType?>, IQueryRequest
    {

    }
}
