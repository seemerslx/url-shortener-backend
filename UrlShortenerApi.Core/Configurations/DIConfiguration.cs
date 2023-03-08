using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using UrlShortenerApi.Core.Interfaces;
using UrlShortenerApi.Core.Services;

namespace UrlShortenerApi.Core.Configurations
{
    public static class DIConfiguration
    {
        public static void RegisterCoreDependencies(this IServiceCollection services, IConfigurationRoot configuration)
        {
            services.AddDbContext<ApplicationDbContext>(opts =>
            {
                opts.UseSqlServer(configuration.GetConnectionString("SQLConnection"));
            });

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddScoped<IUrlService, UrlService>();
            services.AddSingleton<IShortUrlGenerator, ShortUrlGenerator>();
        }
    }
}
