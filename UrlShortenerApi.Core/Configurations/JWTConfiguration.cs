using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortenerApi.Core.Configurations
{
    public class JWTConfiguration
    {
        public string AccessTokenSecret { get; set; } = null!;
        public double AccessTokenExpirationMinutes { get; set; }
        public string Issuer { get; set; } = null!;
        public string Audience { get; set; } = null!;
    }
}
