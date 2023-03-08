namespace UrlShortenerApi.Models
{
    public class UrlManegment : DbItem
    {
        public string Url { get; set; } = null!;
        public string ShortUrl { get; set; } = null!;
    }
}