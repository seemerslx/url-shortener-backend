using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortenerApi.Core.Exceptions
{
    public class BadRequestException : Exception
    {
        public BadRequestException() : base("Bad Request")
        {
        }

        public BadRequestException(string message) : base(message)
        {
        }
        public BadRequestException(string message, Exception innerException) : base(message, innerException)
        {
        }

    }
}
