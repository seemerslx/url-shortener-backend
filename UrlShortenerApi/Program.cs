using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Client;
using System;
using UrlShortenerApi.Core;
using UrlShortenerApi.Core.Configurations;
using UrlShortenerApi.Core.Filters;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers(options =>
{
    options.Filters.Add(typeof(ParseBadRequest));
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddCors(opt =>
{
    opt.AddDefaultPolicy(builderCors =>
    {
        builderCors.WithOrigins(builder.Configuration.GetValue<string>("front-url")).AllowAnyMethod().AllowAnyHeader().
        WithExposedHeaders(new string[] { "totalAmountOfRecords" });
    });
});

builder.Services.RegisterCoreDependencies(builder.Configuration);

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
