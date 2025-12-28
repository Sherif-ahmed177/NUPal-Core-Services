using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Nupal.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class ChatMessage
    {
        [BsonId]
        public ObjectId Id { get; set; }

        // ObjectId string of ChatConversation
        [BsonRepresentation(BsonType.ObjectId)]
        public string ConversationId { get; set; } = default!;

        // Reference to Student.Account.Id
        public string StudentId { get; set; } = default!;

        // "user" | "assistant" | "system"
        public string Role { get; set; } = default!;

        // "rag" | "rl" | "unknown" (used for grouping)
        public string Kind { get; set; } = "unknown";

        public string Content { get; set; } = default!;

        // Optional JSON metadata (e.g., RAG passages, RL slate, confidence)
        public string? MetadataJson { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
