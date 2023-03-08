using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortenerApi.Core.Interfaces
{
    public interface IShortUrlGenerator
    {
        string GenereateShortUrl();
    }
}
