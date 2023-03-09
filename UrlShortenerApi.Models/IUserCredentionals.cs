using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortenerApi.Models
{
    public interface IUserCredentionals
    {
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
