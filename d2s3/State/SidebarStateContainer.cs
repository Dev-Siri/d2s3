using d2s3.Models;

namespace d2s3.State
{
  public class SidebarStateContainer
  {
    private List<ChatThread> _ChatThreads = [];
    public event Action? OnChatThreadsChange;

    public List<ChatThread> ChatThreads
    {
      get => _ChatThreads;
      set
      {
        _ChatThreads = value;
        OnChatThreadsChange?.Invoke();
      }
    }
  }
}