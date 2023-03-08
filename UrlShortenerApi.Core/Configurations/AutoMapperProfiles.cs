using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerApi.Models;
using UrlShortenerApi.Models.DTO_s;

namespace UrlShortenerApi.Core.Configurations
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<UrlManegment, UrlDTO>().ReverseMap();
        }
    }
}
