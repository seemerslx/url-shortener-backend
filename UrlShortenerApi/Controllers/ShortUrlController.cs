using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace UrlShortenerApi.Controllers
{
    [Route("api/short")]
    [ApiController]
    public class ShortUrlController : ControllerBase
    {
        public ActionResult ShortUrl(string url)
        {
            return Ok(url);
        }
    }
}
