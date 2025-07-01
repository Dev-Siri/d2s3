using System.Net.WebSockets;
using System.Text;
using System.Text.Json;
using d2s3.Models;
using Microsoft.AspNetCore.Components;

namespace d2s3.State
{
  public class ChatStateContainer
  {
    private string _CurrentPrompt = "";
    private List<ChatMessage> _CurrentChatMessages = [];
    private bool _IsLoadingAiResponse = false;
    public event Action? OnPromptChange;
    public event Action? OnMessagesChange;
    public event Action? OnAiLoadingResponseChange;
    private readonly ClientWebSocket _WebSocket = new();

    public WebSocket WebSocket
    {
      get => _WebSocket;
    }

    public async Task ConnectWS(string ChatId, bool IsNew, CancellationTokenSource disposalTokenSource)
    {
      if (_WebSocket.State == WebSocketState.Open) return;
      await _WebSocket.ConnectAsync(new Uri($"ws://localhost:5203/chat/{ChatId}?isNew={IsNew}"), disposalTokenSource.Token);
    }

    public async Task DisconectWS(CancellationTokenSource disposalTokenSource)
    {
      if (_WebSocket.State == WebSocketState.Open)
      {
        await _WebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "Bye", disposalTokenSource.Token);
      }
    }

    public async Task HandlePrompt(NavigationManager navigationManager, string? ChatId, bool IsNew)
    {
      var prompt = CurrentPrompt.Trim();

      if (string.IsNullOrEmpty(prompt)) return;

      if (ChatId == null)
      {
        var newChatId = Guid.NewGuid();
        navigationManager.NavigateTo($"/chat/{newChatId}?isNew=true");

        // if this doesnt work, pray, and it might work again 🙏🙏
        while (WebSocket.State != WebSocketState.Open)
        {
          await Task.Delay(100);
        }

        await HandlePrompt(navigationManager, newChatId.ToString(), IsNew);
        return;
      }

      CurrentChatMessages = [..CurrentChatMessages, new ChatMessage
        {
          MessageText = prompt,
          Role = UserRole.User,
          TimeStamp = DateTime.Now,
          MessageType = MessageType.Success
        }
      ];

      CurrentPrompt = "";

      if (_WebSocket != null && _WebSocket.State == WebSocketState.Open)
      {
        IsLoadingAiResponse = true;

        var jsonRequest = JsonSerializer.Serialize(new AiRequest
        {
          Prompt = prompt
        });

        await _WebSocket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(jsonRequest)), WebSocketMessageType.Text, true, CancellationToken.None);

        IsLoadingAiResponse = false;
      }
    }

    public string CurrentPrompt
    {
      get => _CurrentPrompt;
      set
      {
        if (_CurrentPrompt != value)
        {
          _CurrentPrompt = value;
          NotifyPromptStateChanged();
        }
      }
    }

    public bool IsLoadingAiResponse
    {
      get => _IsLoadingAiResponse;
      set
      {
        if (_IsLoadingAiResponse != value)
        {
          _IsLoadingAiResponse = value;
          NotifyAiResponseLoadingStateChanged();
        }
      }
    }

    public List<ChatMessage> CurrentChatMessages
    {
      get => _CurrentChatMessages;
      set
      {
        _CurrentChatMessages = value;
        NotifyMessagesStateChanged();
      }
    }


    private void NotifyPromptStateChanged() => OnPromptChange?.Invoke();
    private void NotifyMessagesStateChanged() => OnMessagesChange?.Invoke();
    private void NotifyAiResponseLoadingStateChanged() => OnAiLoadingResponseChange?.Invoke();
  }
}