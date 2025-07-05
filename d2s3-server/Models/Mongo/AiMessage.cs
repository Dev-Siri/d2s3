using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace d2s3_server.Models.Mongo
{
  public enum AiMessageRole
  {
    Ai,
    User
  }

  public class AiMessage
  {
    [BsonId]
    public required ObjectId Id { get; set; }
    [BsonElement("chatId")]
    [BsonRepresentation(BsonType.String)]
    public required Guid ChatId { get; set; }

    [BsonElement("role")]
    [BsonRepresentation(BsonType.String)]
    public required AiMessageRole Role { get; set; }

    [BsonElement("message")]
    public required string Message { get; set; }

    [BsonElement("createdAt")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
  }
}