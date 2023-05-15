using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Text;
using UrlShortenerApi.Confiuguration;
using UrlShortenerApi.Core;
using UrlShortenerApi.Core.Configurations;
using UrlShortenerApi.Core.Filters;
using UrlShortenerApi.Core.Interfaces;
using UrlShortenerApi.Core.Services;
using UrlShortenerApi.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.RegisterCoreDependencies(builder.Configuration);

builder.Services.RegisterCoreConfiguration(builder.Configuration);

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