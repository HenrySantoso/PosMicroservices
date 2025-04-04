using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Play.Base.Service.Interfaces;

namespace Play.Transaction.Service.Entities
{
    public class Sales : IEntity
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; init; } // ID unik global dari Service Base
        public Guid CustomerId { get; set; } = Guid.Empty; // ID unik global dari Service Base
        public DateTime SaleDate { get; set; } = DateTime.UtcNow;
        public decimal TotalAmount { get; set; } = 0.0m;
    }
}
