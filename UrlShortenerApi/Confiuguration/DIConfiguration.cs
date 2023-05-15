using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;
using UrlShortenerApi.Core.Configurations;
using UrlShortenerApi.Core.Filters;
using UrlShortenerApi.Core.Interfaces;
using UrlShortenerApi.Core.Services;
using UrlShortenerApi.Core;
using Microsoft.EntityFrameworkCore;
using System.IdentityModel.Tokens.Jwt;

namespace UrlShortenerApi.Confiuguration
{
    public static class DIConfiguration
    {
        public static void RegisterCoreDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddControllers(options =>
            {
                options.Filters.Add(typeof(ParseBadRequest));
            });

            services.AddEndpointsApiExplorer();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    In = ParameterLocation.Header,
                    Description = "Please enter your JWT token in the format 'Bearer {token}'",
                    Name = "Authorization",
                    Type = SecuritySchemeType.ApiKey
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement
        {
            {
                new OpenApiSecurityScheme
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                },
                new string[] { }
            }
        });
            });

            services.AddDbContext<ApplicationDbContext>(opts =>
            {
                opts.UseSqlServer(configuration.GetConnectionString("SQLConnection"));
            });

            services.AddIdentity<ApplicationUser, IdentityRole>(opts =>
            {
                opts.User.RequireUniqueEmail = true;
            })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            services.AddTransient<ITokenBuilder, TokenBuilder>();
            services.AddTransient<IUrlService, UrlService>();
            services.AddTransient<IAuthService, AuthService>();
            services.AddSingleton<IShortUrlGenerator, ShortUrlGenerator>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(opts =>
                {
                    JWTConfiguration jwtConfiguration = new JWTConfiguration();
                    configuration.Bind("JWT", jwtConfiguration);

                    opts.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtConfiguration.Issuer,
                        ValidAudience = jwtConfiguration.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtConfiguration.AccessTokenSecret)),
                        ClockSkew = TimeSpan.Zero,
                    };
                });

            services.AddCors(opt =>
            {
                opt.AddDefaultPolicy(builderCors =>
                {
                    builderCors.WithOrigins(configuration.GetValue<string>("front-url")!).AllowAnyMethod().AllowAnyHeader().
                    WithExposedHeaders(new string[] { "totalAmountOfRecords" });
                });
            });
        }

        public static void RegisterCoreConfiguration(this IServiceCollection services, IConfigurationRoot configuration)
        {
            JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();
            services.Configure<JWTConfiguration>(configuration.GetSection("JWT"));
        }
    }
}
