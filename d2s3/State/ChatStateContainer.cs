namespace d2s3.State
{
  public class ChatStateContainer
  {
    private string _CurrentPrompt = "";

    public string CurrentPrompt
    {
      get => _CurrentPrompt;
      set
      {
        if (_CurrentPrompt != value)
        {
          _CurrentPrompt = value;
          NotifyStateChanged();
        }
      }
    }

    public event Action? OnChange;

    private void NotifyStateChanged() => OnChange?.Invoke();
  }
}