using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace TeamSync.Domain.Entities
{
	public class User
	{
		[BsonId] // tells Mongo this is the document's primary key
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; } = string.Empty;
		public string Name { get; set; } = "Unknown";
		public string Email { get; set; } = string.Empty;
		public byte[] PasswordHash { get; set; } = Array.Empty<byte>();
		public byte[] PasswordSalt { get; set; } = Array.Empty<byte>();
	}
}
