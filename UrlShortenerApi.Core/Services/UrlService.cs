using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Core.Helpers;
using UrlShortenerApi.Core.Interfaces;
using UrlShortenerApi.Models;
using UrlShortenerApi.Models.DTO_s;

namespace UrlShortenerApi.Core.Services
{
    public class UrlService : IUrlService
    {
        private readonly ApplicationDbContext context;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly IMapper mapper;
        private IHttpContextAccessor httpContext;

        public UrlService(ApplicationDbContext context,
            IMapper mapper,
            IHttpContextAccessor httpContext,
            UserManager<ApplicationUser> userManager)
        {
            this.context = context;
            this.mapper = mapper;
            this.httpContext = httpContext;
            this.userManager = userManager;
        }

        public async Task<UrlDTO?> GetUrlById(Guid id)
        {
            var url = await context.Urls.Include(u => u.User).FirstOrDefaultAsync(x => x.Id == id);

            if (url == null)
            {
                return null;
            }
            var mapperUrl = mapper.Map<UrlDTO>(url);

            mapperUrl.CreatedBy = url.User.Email!;

            return mapperUrl;
        }

        public async Task<UrlDTO> AddUrl(UrlRequest url, string randomString, string userEmail)
        {
            var user = await userManager.FindByEmailAsync(userEmail);

            var sUrl = new UrlManegment()
            {
                Id = Guid.NewGuid(),
                Url = url.Url,
                ShortUrl = randomString,
                CreatedAt = DateTime.Now,
                User = user!
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

        public async Task<bool> RemoveUrlWithCheckingCreator(Guid urlId, string email)
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return false;

            var res = await context.Urls.FirstOrDefaultAsync(x => x.Id == urlId);

            if (res == null)
                return false;

            if (res.UserId != user.Id)
                return false;

            context.Urls.Remove(res);
            await context.SaveChangesAsync();

            return true;
        }
    }
}
