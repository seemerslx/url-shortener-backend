using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UrlShortenerApi.Core.Configurations;
using UrlShortenerApi.Core.Constants;
using UrlShortenerApi.Core.Interfaces;
using UrlShortenerApi.Models;
using UrlShortenerApi.Models.DTO_s;

namespace UrlShortenerApi.Core.Services
{
    public class TokenBuilder : ITokenBuilder
    {
        private readonly UserManager<ApplicationUser> userManager;

        public TokenBuilder(UserManager<ApplicationUser> userManager)
        {
            this.userManager = userManager;
        }

        public async Task<AuthenticationResponse> BuildToken(IUserCredentionals userCredentionals,
            JWTConfiguration configuration)
        {
            var claims = new List<Claim>()
            {
                new Claim(ClaimsConstants.Email, userCredentionals.Email),
            };

            if (userCredentionals is RegisterRequest registerRequest)
            {
                claims.Add(new Claim(ClaimsConstants.Usernmae, registerRequest.Username));
            }

            var user = await userManager.FindByEmailAsync(userCredentionals.Email);
            var claimsList = await userManager.GetClaimsAsync(user!);

            foreach (var claim in claimsList)
            {
                claims.Add(claim);
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration.AccessTokenSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.Now.AddMinutes(configuration.AccessTokenExpirationMinutes);

            var token = new JwtSecurityToken(issuer: configuration.Issuer,
                audience: configuration.Audience,
                expires: expiration,
                signingCredentials: creds,
                claims: claims);

            var response = new AuthenticationResponse()
            {
                Token = new JwtSecurityTokenHandler().WriteToken(token),
                Expiration = expiration
            };

            return response;
        }
    }
}
