var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.MapFallback((HttpContext context) =>
{
    var path = context.Request.Path.ToUriComponent().Trim('/');
    string urlMatch = "https://www.youtube.com/watch?v=WIWfNCoDiu0&ab_channel=MohamadLawand";

    if (urlMatch is null)
    {
        return Results.BadRequest("Invalid short url");
    }

    return Results.Redirect(url: urlMatch);
});

app.Run();
