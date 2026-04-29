using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
});

var app = builder.Build();

app.MapGet("/health", () => Results.Ok(new { status = "ok" }));

app.MapGet("/api/meta", () => Results.Ok(new
{
    name = "SpellCaster: Arena API",
    version = "v0",
    stack = "C# ASP.NET Core Minimal API"
}));

app.Run();
