using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UrlShortenerApi.Core.Exceptions
{
    public class IdentityException : Exception
    {
        public IEnumerable<IdentityError> Errors { get; set; }

        public IdentityException(IEnumerable<IdentityError> errors)
        {
            Errors = errors;
        }
    }
}
