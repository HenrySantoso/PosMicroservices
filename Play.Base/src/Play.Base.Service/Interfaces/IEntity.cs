using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Play.Base.Service.Interfaces
{
    public interface IEntity
    {
        [BsonId]
        [BsonGuidRepresentation(GuidRepresentation.Standard)]
        public Guid Id { get; init; }
    }
}
