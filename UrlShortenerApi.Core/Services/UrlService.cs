using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerApi.Core.Helpers;
using UrlShortenerApi.Core.Interfaces;
using UrlShortenerApi.Models;
using UrlShortenerApi.Models.DTO_s;

namespace UrlShortenerApi.Core.Services
{
    public class UrlService : IUrlService
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;
        private IHttpContextAccessor httpContext;

        public UrlService(ApplicationDbContext context, IMapper mapper, IHttpContextAccessor httpContext)
        {
            this.context = context;
            this.mapper = mapper;
            this.httpContext = httpContext;
        }

        public async Task<UrlDTO?> GetUrlById(Guid id)
        {
            var url = await context.Urls.FirstOrDefaultAsync(x => x.Id == id);

            if (url == null)
            {
                return null;
            }

            return mapper.Map<UrlDTO>(url);
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

        public async Task<bool> RemoveUrl(Guid urlId)
        {
            var res = await context.Urls.FirstOrDefaultAsync(x => x.Id == urlId);
            if (res == null)
            {
                return false;
            }

            context.Urls.Remove(res);
            await context.SaveChangesAsync();
            
            return true;
        }

        public async Task<List<UrlDTO>> GetUrls(PaginationDTO paginationDTO)
        {
            var queriable = context.Urls.AsQueryable();
            await httpContext.HttpContext.InsertParametersPaginationInHeader(queriable);
            var urls = await queriable.Paginate(paginationDTO).ToListAsync();

            return mapper.Map<List<UrlDTO>>(urls);
        }
    }
}
