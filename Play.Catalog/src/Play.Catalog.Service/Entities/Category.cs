using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Play.Catalog.Service.Entities
{
    public class Category
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid CategoryId { get; init; }
        public string CategoryName { get; init; } = string.Empty;
    }
}
