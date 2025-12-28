using System.Text.Json.Serialization;

namespace NUPAL.Core.Application.DTOs
{
    public class ChatSendRequestDto
    {
        [JsonPropertyName("conversation_id")]
        public string? ConversationId { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; } = string.Empty;

        // Optional: let the UI hint the language (agent still detects)
        [JsonPropertyName("lang")]
        public string? Lang { get; set; }
    }

    public class ChatReplyDto
    {
        [JsonPropertyName("kind")]
        public string Kind { get; set; } = "unknown";

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        // Optional metadata (serialized JSON) returned by agent
        [JsonPropertyName("metadata_json")]
        public string? MetadataJson { get; set; }
    }

    public class ChatSendResponseDto
    {
        [JsonPropertyName("conversation_id")]
        public string ConversationId { get; set; } = string.Empty;

        [JsonPropertyName("replies")]
        public List<ChatReplyDto> Replies { get; set; } = new();
    }
}
