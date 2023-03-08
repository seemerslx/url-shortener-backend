using Microsoft.EntityFrameworkCore;
using UrlShortenerApi.Core;
using UrlShortenerApi.Core.Configurations;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.RegisterCoreDependencies(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

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
