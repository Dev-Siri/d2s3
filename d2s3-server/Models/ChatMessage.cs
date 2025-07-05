namespace d2s3_server.Models
{
  public enum UserRole
  {
    User,
    Ai
  }

  public enum MessageType
  {
    Success,
    Failure
  }

  public class ChatMessage
  {
    public required MessageType MessageType { get; set; }
    public required UserRole Role { get; set; }
    public required string MessageText { get; set; }
    public required DateTime TimeStamp { get; set; }
  }
}