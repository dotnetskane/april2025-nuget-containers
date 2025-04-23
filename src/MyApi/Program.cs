using MyPackage;
using Scalar.AspNetCore;
using System.Runtime.InteropServices;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
    app.MapScalarApiReference("/");
}

app.UseHttpsRedirection();

app.MapWeatherEndpoints();
app.MapGet("/info", () =>
{
    return TypedResults.Json(new
    {
        RuntimeInformation.FrameworkDescription,
        RuntimeInformation.RuntimeIdentifier,
        OSArchitecture = RuntimeInformation.OSArchitecture.ToString(),
        OSVersion = Environment.OSVersion.ToString()
    });
})
.Produces(StatusCodes.Status200OK);

app.Run();
