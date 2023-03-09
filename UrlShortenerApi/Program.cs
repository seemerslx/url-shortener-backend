using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using UrlShortenerApi.Core;
using UrlShortenerApi.Core.Configurations;
using UrlShortenerApi.Core.Filters;
using UrlShortenerApi.Core.Interfaces;
using UrlShortenerApi.Core.Services;
using UrlShortenerApi.Models;

var builder = WebApplication.CreateBuilder(args);

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ParseBadRequest));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
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


builder.Services.AddDbContext<ApplicationDbContext>(opts =>
{
    opts.UseSqlServer(builder.Configuration.GetConnectionString("SQLConnection"));
});

builder.Services.AddIdentity<ApplicatonUser, IdentityRole>(opts =>
{
    opts.User.RequireUniqueEmail = true;
})
    .AddEntityFrameworkStores<ApplicationDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

builder.Services.AddScoped<ITokenBuilder, TokenBuilder>();
builder.Services.AddScoped<IUrlService, UrlService>();
builder.Services.AddSingleton<IShortUrlGenerator, ShortUrlGenerator>();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(opts =>
    {
        JWTConfiguration jwtConfiguration = new JWTConfiguration();
        builder.Configuration.Bind("JWT", jwtConfiguration);

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

builder.Services.Configure<JWTConfiguration>(builder.Configuration.GetSection("JWT"));

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builderCors =>
    {
        builderCors.WithOrigins(builder.Configuration.GetValue<string>("front-url")).AllowAnyMethod().AllowAnyHeader().
        WithExposedHeaders(new string[] { "totalAmountOfRecords" });
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors();

app.UseAuthorization();
app.UseAuthentication();

app.MapControllers();

app.MapFallback(async (HttpContext context, ApplicationDbContext dbContext) =>
{
    var path = context.Request.Path.ToUriComponent().Trim('/');

    var urlMatch = await dbContext.Urls
    .FirstOrDefaultAsync(x => x.ShortUrl.Trim() == path.Trim());

    if (urlMatch is null)
    {
        return Results.BadRequest("Invalid short url");
    }

    return Results.Redirect(url: urlMatch.Url);
});

app.Run();
