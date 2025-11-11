using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace TeamSync.Domain.Entities
{
	public class Project
	{
		[BsonId]
		[BsonRepresentation(BsonType.ObjectId)]
		public string Id { get; set; } = string.Empty;

		public string Name { get; set; } = string.Empty;

		public string Description { get; set; } = string.Empty;

		public string OwnerId { get; set; } = string.Empty;

		public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
	}
}
