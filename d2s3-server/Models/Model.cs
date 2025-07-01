using System.Text.Json.Serialization;

namespace d2s3_server.Models
{
    public class ModelContentPart
    {
        [JsonPropertyName("text")]
        public required string Text { get; set; }
    }

    public class OneModelMessage
    {
        [JsonPropertyName("role")]
        public required string Role { get; set; }
        [JsonPropertyName("parts")]
        public required ModelContentPart[] Parts { get; set; }
    }

    public class GeminiRequest
    {
        [JsonPropertyName("contents")]
        public required List<OneModelMessage> Contents { get; set; }
    }
}