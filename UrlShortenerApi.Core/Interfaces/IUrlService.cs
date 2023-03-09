using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerApi.Models;
using UrlShortenerApi.Models.DTO_s;

namespace UrlShortenerApi.Core.Interfaces
{
    public interface IUrlService
    {
        Task<bool> RemoveUrl(Guid urlId);
        Task<bool> RemoveUrlWithCheckingCreator(Guid urlId, string email);

        Task<bool> CheckUrlExists(UrlRequest urlDTO);

        Task<UrlDTO> AddUrl(UrlRequest urlDTO, string randomSting, string userEmail);

        Task<List<UrlDTO>> GetUrls(PaginationDTO paginationDTO);

        Task<UrlDTO?> GetUrlById(Guid id);
    }
}
