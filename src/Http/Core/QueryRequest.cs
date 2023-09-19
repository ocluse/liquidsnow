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
        public IEnumerable<string>? Ids { get; set; }

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
    public record QueryRequest : QueryRequest<QueryType?>, IQueryRequest
    {

    }
}
