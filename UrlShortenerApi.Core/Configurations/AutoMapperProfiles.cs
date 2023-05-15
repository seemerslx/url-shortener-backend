using AutoMapper;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerApi.Models.DTO_s;

namespace UrlShortenerApi.Core.Configurations
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UrlManegment, UrlDTO>()
                    .ForMember(dest => dest.ShortUrl, opt => opt.MapFrom<ShortUrlResolver>());
        }



        



    }

    public class ShortUrlResolver : IValueResolver<UrlManegment, UrlDTO, string>
    {
        private readonly HttpContext _httpContext;

        public ShortUrlResolver(IHttpContextAccessor httpContextAccessor)
        {
            _httpContext = httpContextAccessor.HttpContext;
        }

        public string Resolve(UrlManegment source, UrlDTO destination, string destMember, ResolutionContext context)
        {
            return $"{_httpContext.Request.Scheme}://{_httpContext.Request.Host}/{source.ShortUrl}";
        }
    }
}
