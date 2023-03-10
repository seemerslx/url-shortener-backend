using UrlShortenerApi.Models;

namespace UrlShortenerApi.Core
{
    public class UrlManegment : DbItem
    {
        public string Url { get; set; } = null!;
        public string ShortUrl { get; set; } = null!;
        public DateTime CreatedAt { get; set; }
        public string UserId { get; set; } = null!;
        public ApplicationUser User { get; set; } = null!;

    }
}