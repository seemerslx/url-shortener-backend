using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerApi.Core.Interfaces;

namespace UrlShortenerApi.Core.Services
{
    public class ShortUrlGenerator : IShortUrlGenerator
    {
        public string GenereateShortUrl()
        {
            var random = new Random();

            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890@az";

            var randomString = new string(Enumerable.Repeat(chars, 16)
                .Select(x => x[random.Next(x.Length)])
                .ToArray()
            );

            return randomString;
        }
    }
}
