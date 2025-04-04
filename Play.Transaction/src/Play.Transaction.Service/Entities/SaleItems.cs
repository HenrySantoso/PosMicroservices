using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Play.Base.Service.Interfaces;

namespace Play.Transaction.Service.Entities
{
    public class SaleItems : IEntity
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; init; }
        public Guid ProductId { get; set; } = Guid.Empty;
        public Guid SaleId { get; set; } = Guid.Empty;
        public int Quantity { get; set; } = 0;
        public decimal Price { get; set; } = 0.0m;
    }
}
