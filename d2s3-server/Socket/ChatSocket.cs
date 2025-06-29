using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using d2s3_server.Models;
using MongoDB.Driver;

namespace d2s3_server.Socket
{
  class ChatSocket
  {
    public static async Task HandleWSMessages(HttpContext context, WebSocket webSocket, MongoClient mongo, Guid chatId, bool isNew)
    {
      List<string> messages = [];

      if (!isNew)
      {
        // TODO: fetch messages
      }

      var buffer = new byte[1024 * 4];
      var result = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);

      if (result.MessageType == WebSocketMessageType.Text)
      {
        var json = Encoding.UTF8.GetString(buffer, 0, result.Count);

        Console.WriteLine(json);
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