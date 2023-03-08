using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortenerApi.Models.DTO_s
{
    public class UrlDTO : DbItem
    {
        public string Url { get; set; } = null!;
        public string ShortUrl { get; set; } = null!;
    }
}
