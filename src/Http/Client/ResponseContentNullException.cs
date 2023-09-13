using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ocluse.LiquidSnow.Http.Client
{
    /// <summary>
    /// An exception that is thrown when the response content is null and the request was expecting content.
    /// </summary>
    public class ResponseContentNullException : Exception
    {
        /// <summary>
        /// Creates a new instance of <see cref="ResponseContentNullException"/>
        /// </summary>
        public ResponseContentNullException()
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="ResponseContentNullException"/>
        /// </summary>
        public ResponseContentNullException(string message) : base(message)
        {
        }

        /// <summary>
        /// Creates a new instance of <see cref="ResponseContentNullException"/>
        /// </summary>
        public ResponseContentNullException(string message, Exception innerException) : base(message, innerException)
        {
        }
    }
}
