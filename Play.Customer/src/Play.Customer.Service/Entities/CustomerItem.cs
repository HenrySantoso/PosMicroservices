using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using Play.Base.Service.Interfaces;

namespace Play.Customer.Service.Entities
{
    public class CustomerItem : IEntity
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; init; } // ID unik global dari Service Base
        public int CustomerId { get; init; } // ID unik dalam Customer Service
        public string CustomerName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Address { get; set; } = string.Empty;
    }
}
