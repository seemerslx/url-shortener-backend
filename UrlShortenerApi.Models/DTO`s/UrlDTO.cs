using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortenerApi.Models.DTO_s
{
    public class UrlDTO : DbItem
    {
        public string Url { get; set; } = null!;
        public string ShortUrl { get; set; } = null!;

        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = null!;
        public string CreatedBy { get; set; } = null!;
    }
}
