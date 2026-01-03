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

    public class ChatConversationDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("title")]
        public string Title { get; set; } = string.Empty;

        [JsonPropertyName("last_activity_at")]
        public DateTime LastActivityAt { get; set; }
        [JsonPropertyName("is_pinned")]
        public bool IsPinned { get; set; }
    }

    public class ChatMessageDto
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = string.Empty;

        [JsonPropertyName("role")]
        public string Role { get; set; } = string.Empty; 

        [JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [JsonPropertyName("created_at")]
        public DateTime CreatedAt { get; set; }
    }
}
