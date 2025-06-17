using d2s3.Models;

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