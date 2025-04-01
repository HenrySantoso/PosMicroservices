using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Play.Catalog.Service.Entities
{
    public class Product
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        Guid ProductId { get; init; }
        string ProductName { get; set; } = string.Empty;
        decimal Price { get; set; } = 0.0m;
        int StockQuantity { get; set; } = 0;
        string Description { get; set; } = string.Empty;
    }
}
