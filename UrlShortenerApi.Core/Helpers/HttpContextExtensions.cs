using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UrlShortenerApi.Core.Helpers
{
    public static class HttpContextExtensions
    {
        public async static Task InsertParametersPaginationInHeader<T>(this HttpContext context,
            IQueryable<T> quariable)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            double count = await quariable.CountAsync();

            context.Response.Headers.Append("totalAmountOfRecords", count.ToString());
        }
    }
}
