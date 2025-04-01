using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Play.Catalog.Service.Entities
{
    public class Product
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        Guid ProductId { get; init; } = null!;
        string ProductName { get; set; } = string.Empty;
        decimal Price { get; set; } = 0.0m;
        int StockQuantity { get; set; } = 0;
        string Description { get; set; } = string.Empty;
    }
}
