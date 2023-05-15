using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections;
using System.Security.Claims;
using UrlShortenerApi.Core;
using UrlShortenerApi.Core.Constants;
using UrlShortenerApi.Core.Interfaces;
using UrlShortenerApi.Models;
using UrlShortenerApi.Models.DTO_s;

namespace UrlShortenerApi.Controllers
{
    [Route("api/shorturl")]
    [ApiController]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ShortUrlController : ControllerBase
    {
        

        
        private readonly IShortUrlGenerator shortUrlGenerator;
        private readonly IUrlService urlService;

        public ShortUrlController(IShortUrlGenerator shortUrlGenerator,
            IUrlService urlService)
        {
            this.shortUrlGenerator = shortUrlGenerator;
            this.urlService = urlService;
        }

        [HttpPost]
        public async Task<IActionResult> ShortUrl(UrlRequest url)
        {
            if (!Uri.TryCreate(url.Url, UriKind.Absolute, out var inputUrl))
            {
                return BadRequest("Invalid Url has been provided");
            }

            if (await urlService.CheckUrlExists(url))
            {
                return BadRequest("Such Url already exists");
            }

            var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimsConstants.Email);

            var randomString = shortUrlGenerator.GenereateShortUrl();

            var sUrl = await urlService.AddUrl(url, randomString, email!.Value);

            var response = $"{HttpContext.Request.Scheme}://{HttpContext.Request.Host}/{sUrl.ShortUrl}";

            return Ok(new UrlResponse { Url = response });
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetUrls([FromQuery] PaginationDTO paginationDTO)
        {
            var urls = await urlService.GetUrls(paginationDTO);
            return Ok(urls);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetUrlById(Guid id)
        {
            var url = await urlService.GetUrlById(id);

            if (url != null)
            {
                return Ok(url);
            }

            return BadRequest("Url not found");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveUrl(Guid id)
        {
            var res = HttpContext.User.Claims.Where(x => x.Type == ClaimsConstants.Role);

            if (res != null)
            {
                if (res.Any(x => x.Value == RolesConstants.Admin ))
                {
                    // check
                    if (await urlService.RemoveUrl(id))
                    {
                        return NoContent();
                    }

                    return BadRequest("Url with such id not found");
                }
                else
                {
                    var email = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimsConstants.Email);

                    if (await urlService.RemoveUrlWithCheckingCreator(id, email!.Value))
                    {
                        return NoContent();
                    }
                    return BadRequest("You are not allowed to delete this url!");
                }
            }
            return BadRequest("Somethin went wrong");

        }
    }
}
