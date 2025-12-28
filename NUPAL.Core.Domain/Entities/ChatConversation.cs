using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Nupal.Domain.Entities
{
    [BsonIgnoreExtraElements]
    public class ChatConversation
    {
        [BsonId]
        public ObjectId Id { get; set; }

        // Reference to Student.Account.Id
        public string StudentId { get; set; } = default!;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public DateTime LastActivityAt { get; set; } = DateTime.UtcNow;
    }
}
