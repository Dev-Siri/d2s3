using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using d2s3_server.Models;
using d2s3_server.Services;
using MongoDB.Driver;

namespace d2s3_server.Socket
{
  class ChatSocket
  {
    public static async Task HandleWSMessages(HttpContext context, WebSocket webSocket, MongoClient mongo, GeminiService geminiService, Guid chatId, bool isNew)
    {
      var messages = new List<OneModelMessage>();

      if (!isNew)
      {
        // mongo. 
      }

      while (webSocket.State == WebSocketState.Open)
      {
        var buffer = new byte[1024 * 4];
        var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

        if (result.MessageType == WebSocketMessageType.Text)
        {
          var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

          if (json == null)
          {
            await SendAiResponse(webSocket, new Response
            {
              Success = false,
              ResponseMessage = "Invalid request format."
            });
            continue;
          }

          var aiRequest = JsonSerializer.Deserialize<AiRequest>(json);

          if (aiRequest == null || string.IsNullOrWhiteSpace(aiRequest.Prompt))
          {
            await SendAiResponse(webSocket, new Response
            {
              Success = false,
              ResponseMessage = "Prompt cannot be empty."
            });
            continue;
          }

          messages.Add(new OneModelMessage
          {
            Role = "user",
            Parts =
            [
              new ModelContentPart { Text = aiRequest.Prompt }
            ]
          });

          var response = await geminiService.GenerateTextAsync(messages);

          messages.Add(new OneModelMessage
          {
            Role = "model",
            Parts =
            [
              new ModelContentPart { Text = response.ResponseMessage }
            ]
          });

          await SendAiResponse(webSocket, response);
        }
      }
    }

    private static async Task SendAiResponse(WebSocket socket, Response response)
    {
      var json = JsonSerializer.Serialize(response);
      var buffer = Encoding.UTF8.GetBytes(json);
      var segment = new ArraySegment<byte>(buffer);

      await socket.SendAsync(
          segment,
          WebSocketMessageType.Text,
          endOfMessage: true,
          cancellationToken: CancellationToken.None
      );
    }
  }
}