using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Backend.Models
{
    public class UsersModel
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; }

        [BsonElement("name")]
        public string Name { get; set; }

        [BsonElement("email")]
        public string Email { get; set; }

        [BsonElement("banner")]
        public string Banner { get; set; }

        [BsonElement("password")]
        public string Password { get; set; }

        [BsonElement("favoritos")]
        public List<string> Favoritos { get; set; } = new(); // IDs de restaurantes
    }
}
