using d2s3_server.Models;
using d2s3_server.Models.Mongo;
using d2s3_server.Services;
using d2s3_server.Socket;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Services.AddScoped<GeminiService>();
builder.Services.AddSingleton(_ =>
{
    var connectionString = Environment.GetEnvironmentVariable("MONGODB_URI");

    if (connectionString == null)
    {
        Console.Error.WriteLine("ERR: Missing environment variable 'MONGODB_URI'");
        Environment.Exit(0);
    }

    return new MongoClient(connectionString);
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

var webSocketOptions = new WebSocketOptions
{ KeepAliveInterval = TimeSpan.FromMinutes(2) };

app.UseWebSockets(webSocketOptions);
app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.Map("/chat", context =>
{
    var newChatId = Guid.NewGuid();
    context.Response.Redirect($"/chat/{newChatId}?isNew=true");
    return Task.CompletedTask;
});

app.Map("/chat/{id:guid}", async (Guid id, [FromQuery] bool isNew, HttpContext context, MongoClient mongo, GeminiService geminiService) =>
{
    if (context.WebSockets.IsWebSocketRequest)
    {
        using var webSocket = await context.WebSockets.AcceptWebSocketAsync();

        await ChatSocket.HandleWSMessages(webSocket, mongo, geminiService, id, isNew);
    }
    else
    {
        context.Response.StatusCode = 405;
    }
});

app.MapGet("/chat/{id:guid}/messages", (Guid id, HttpContext context, MongoClient mongo) =>
{
    var db = mongo.GetDatabase("d2s3");
    var initialChats = db
        .GetCollection<AiMessage>("messages")
        .Find(c => c.ChatId == id).ToList();
    List<ChatMessage> structuredChats = [];

    foreach (var chat in initialChats)
    {
        structuredChats.Add(new ChatMessage
        {
            MessageText = chat.Message,
            // I really gave up at the end of making a proper enum for this
            Role = chat.Role == AiMessageRole.Ai ? UserRole.Ai : UserRole.User,
            MessageType = MessageType.Success,
            TimeStamp = chat.CreatedAt,
        });
    }

    return Results.Json(structuredChats);
});

app.MapGet("/chat/title", async ([FromQuery] string prompt, GeminiService geminiService) =>
{
    var title = await geminiService.GenerateTitle(prompt);
    return Results.Json(title);
});

app.Run();
