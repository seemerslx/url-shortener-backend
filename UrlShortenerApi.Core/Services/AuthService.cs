using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using UrlShortenerApi.Core.Configurations;
using UrlShortenerApi.Core.Constants;
using UrlShortenerApi.Core.Exceptions;
using UrlShortenerApi.Core.Interfaces;
using UrlShortenerApi.Models;
using UrlShortenerApi.Models.DTO_s;

namespace UrlShortenerApi.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly ITokenBuilder tokenBuilder;
        private readonly ApplicationDbContext context;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AuthService(UserManager<ApplicationUser> userManager,
            ITokenBuilder tokenBuilder,
            ApplicationDbContext context, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.tokenBuilder = tokenBuilder;
            this.context = context;
            this.signInManager = signInManager;
        }

        public async Task<AuthenticationResponse> LoginUser(LoginRequest loginCredentionals, JWTConfiguration jWTConfiguration)
        {
            ApplicationUser? signedUser = await userManager.FindByEmailAsync(loginCredentionals.Email);
            
            if (signedUser == null)
            {
                throw new BadRequestException("User not registered!");
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
                throw new BadRequestException("Invalid credentionals");
            }
        }

        public async Task<AuthenticationResponse> RegisterUser(RegisterRequest registerCredentionals,
            JWTConfiguration jWTConfiguration)
        {
            if (await context.Users.AnyAsync(x => (x.Email == registerCredentionals.Email)))
            {
                throw new BadRequestException("This email address is already taken");
            }

            var user = new ApplicationUser { UserName = registerCredentionals.Username, Email = registerCredentionals.Email };
            
            var result = await userManager.CreateAsync(user, registerCredentionals.Password);

            if (result.Succeeded)
            {
                await userManager.AddClaimAsync(user, new Claim(ClaimsConstants.Role, RolesConstants.User));
                return await tokenBuilder.BuildToken(registerCredentionals, jWTConfiguration);
            }
            else
            {
                throw new IdentityException(result.Errors);
            }
        }
    }
}