using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerApi.Core.Interfaces;
using UrlShortenerApi.Models;
using UrlShortenerApi.Models.DTO_s;

namespace UrlShortenerApi.Core.Services
{
    public class UrlService : IUrlService
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public UrlService(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        public async Task<UrlDTO> AddUrl(UrlRequest url, string randomString)
        {
            var sUrl = new UrlManegment()
            {
                Id = Guid.NewGuid(),
                Url = url.Url,
                ShortUrl = randomString
            };

            await context.AddAsync(sUrl);

            await context.SaveChangesAsync();

            return mapper.Map<UrlDTO>(sUrl);
        }

        public async Task<bool> CheckUrlExists(UrlRequest url)
        {
            return await context.Urls.AnyAsync(x => x.Url == url.Url);
        }

        public Task<bool> RemoveUrl(Guid urlId)
        {
            throw new NotImplementedException();
        }
    }
}
