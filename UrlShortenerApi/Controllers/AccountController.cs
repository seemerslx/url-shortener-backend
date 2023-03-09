using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using UrlShortenerApi.Core;
using UrlShortenerApi.Core.Configurations;
using UrlShortenerApi.Core.Interfaces;
using UrlShortenerApi.Core.Services;
using UrlShortenerApi.Models.DTO_s;
using System.Security.Claims;
using UrlShortenerApi.Core.Constants;

namespace UrlShortenerApi.Controllers
{
    [ApiController]
    [Route("api/accounts")]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<ApplicatonUser> userManager;
        private readonly SignInManager<ApplicatonUser> signInManager;
        private readonly ApplicationDbContext context;
        private readonly JWTConfiguration jWTConfiguration;
        private readonly ITokenBuilder tokenBuilder;

        public AccountController(UserManager<ApplicatonUser> userManager,
            SignInManager<ApplicatonUser> signInManager,
             IOptions<JWTConfiguration> options,
             ITokenBuilder tokenBuilder,
             ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.jWTConfiguration = options.Value;
            this.tokenBuilder = tokenBuilder;
            this.context = context;
        }

        [HttpPost("register")]
        public async Task<ActionResult<AuthenticationResponse>> Register(
            [FromBody] RegisterRequest registerCredentionals)
        {
            // checks for existing email
            if (await context.Users.AnyAsync(x => (x.Email == registerCredentionals.Email)))
            {
                return BadRequest("This email already registered");
            }

            var user = new ApplicatonUser { UserName = registerCredentionals.Username, Email = registerCredentionals.Email };

            // if username exists here will be thrown error and handlaed in my filter
            var result = await userManager.CreateAsync(user, registerCredentionals.Password);


            if (result.Succeeded)
            {
                await userManager.AddClaimAsync(user, new Claim(ClaimsConstants.Role, RolesConstants.User));
                return await tokenBuilder.BuildToken(registerCredentionals, jWTConfiguration);
            }
            else
            {
                return BadRequest(result.Errors);
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult<AuthenticationResponse>> Login(
            LoginRequest loginCredentionals)
        {
            ApplicatonUser? signedUser = await userManager.FindByEmailAsync(loginCredentionals.Email);
            if (signedUser == null)
            {
                return BadRequest("User not registerd!");
            }

            var result = await signInManager.PasswordSignInAsync(signedUser.UserName,
                loginCredentionals.Password, 
                isPersistent: false,
                lockoutOnFailure: false);

            if (result.Succeeded)
            {
                return await tokenBuilder.BuildToken(loginCredentionals, jWTConfiguration);
            }
            else
            {
                return BadRequest("Invalid credentionals");
            }
        }

    }
}
