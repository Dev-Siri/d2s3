using d2s3_server.Models;
using d2s3_server.Services;
using Microsoft.AspNetCore.Mvc;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddScoped<GeminiService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapPost("/gemini", async ([FromBody] AiRequest request, GeminiService geminiService) =>
{
    var result = await geminiService.GenerateTextAsync(request.Prompt);
    return Results.Json(result);
})
.WithName("GenerateTextGemini");

app.Run();
