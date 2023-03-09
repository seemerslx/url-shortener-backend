using Microsoft.AspNetCore.Mvc;
using UrlShortenerApi.Core.Configurations;
using UrlShortenerApi.Core.Interfaces;
using UrlShortenerApi.Models.DTO_s;
using UrlShortenerApi.Core.Exceptions;
using Microsoft.Extensions.Options;

namespace UrlShortenerApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly JWTConfiguration jWTConfiguration;
        private readonly IAuthService authService;

        public AccountController(IOptions<JWTConfiguration> options, IAuthService authService)
        {
            this.jWTConfiguration = options.Value;
            this.authService = authService;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthenticationResponse>> Register(
            [FromBody] RegisterRequest registerCredentionals)
        {
            try
            {
                return await authService.RegisterUser(registerCredentionals, jWTConfiguration);
            }
            catch (Exception ex)
            {
                if (ex is IdentityException identityException)
                {
                    return BadRequest(identityException.Errors);
                }

                return BadRequest(ex.Message);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> Login(
            LoginRequest loginCredentionals)
        {
            try
            {
                return await authService.LoginUser(loginCredentionals, jWTConfiguration);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

    }
}