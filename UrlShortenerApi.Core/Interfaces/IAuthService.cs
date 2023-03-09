using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerApi.Core.Configurations;
using UrlShortenerApi.Models;
using UrlShortenerApi.Models.DTO_s;

namespace UrlShortenerApi.Core.Interfaces
{
    public interface IAuthService
    {
        Task<AuthenticationResponse> RegisterUser(RegisterRequest registerCredentionals,
            JWTConfiguration jWTConfiguration);

        Task<AuthenticationResponse> LoginUser(LoginRequest loginRequest,
            JWTConfiguration jWTConfiguration);
    }
}
