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
    public interface ITokenBuilder
    {
        Task<AuthenticationResponse> BuildToken(IUserCredentionals userCredentionals, JWTConfiguration configuration);
    }
}
