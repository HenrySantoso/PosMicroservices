using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Play.Base.Service.Interfaces;

namespace Play.Catalog.Service.Entities
{
    public class Product : IEntity
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; init; }
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; } = 0.0m;
        public int StockQuantity { get; set; } = 0;
        public string Description { get; set; } = string.Empty;
        public Guid CategoryId { get; set; } = Guid.Empty;
    }
}
