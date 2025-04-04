using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Play.Base.Service.Interfaces;

namespace Play.Catalog.Service.Entities
{
    public class Category : IEntity
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; init; }
        public string CategoryName { get; set; } = string.Empty;
    }
}
