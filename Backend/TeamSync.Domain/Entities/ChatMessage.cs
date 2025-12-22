using MongoDB.Bson.Serialization.Attributes;

namespace TeamSync.Domain.Entities
{
    public class ChatMessage : BaseEntity
    {
        [BsonElement("projectId")]
        public string ProjectId { get; set; } = default!;

        [BsonElement("senderId")]
        public string SenderId { get; set; } = default!;

        [BsonElement("senderName")]
        public string SenderName { get; set; } = default!;

        [BsonElement("message")]
        public string Message { get; set; } = default!;
    }
}
