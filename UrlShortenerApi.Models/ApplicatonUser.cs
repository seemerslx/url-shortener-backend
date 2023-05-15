using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UrlShortenerApi.Models;

namespace UrlShortenerApi.Core
{
    public class ApplicationUser : IdentityUser
    {
        public ICollection<UrlManegment> Urls { get; set; } = null!;


    }
}
