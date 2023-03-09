using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortenerApi.Core.Exceptions
{
    public class NotFoundException : Exception
    {
        public NotFoundException() : base("Not found")
        {
            
        }

        public NotFoundException(string message) : base(message)
        {
            
        }
    }
}
