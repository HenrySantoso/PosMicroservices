using MongoDB.Bson;
using MongoDB.Driver;

namespace Play.Customer.Service.Services
{
    public class CounterService
    {
        private readonly IMongoCollection<BsonDocument> _counterCollection;

        public CounterService(IMongoDatabase database)
        {
            _counterCollection = database.GetCollection<BsonDocument>("counters");
        }

        public async Task<int> GetNextCustomerIdAsync()
        {
            var filter = Builders<BsonDocument>.Filter.Eq("_id", "customerid");
            var update = Builders<BsonDocument>.Update.Inc("sequence_value", 1);
            var options = new FindOneAndUpdateOptions<BsonDocument>
            {
                ReturnDocument = ReturnDocument.After,
                IsUpsert = true
            };

            var result = await _counterCollection.FindOneAndUpdateAsync(filter, update, options);
            return result["sequence_value"].AsInt32;
        }
    }
}
