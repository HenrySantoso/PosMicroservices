using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Play.Base.Service.Interfaces;
using Play.Base.Service.Settings;

namespace Play.Base.Service.MongoDB
{
    public static class Exstensions
    {
        public static IServiceCollection AddMongo(this IServiceCollection services)
        {
            services.AddSingleton(serviceProvider =>
            {
                var configuration = serviceProvider.GetRequiredService<IConfiguration>();
                var serviceSettings = configuration
                    .GetSection(nameof(ServiceSettings))
                    .Get<ServiceSettings>();
                var mongoDbSettings = configuration
                    .GetSection(nameof(MongoDbSettings))
                    .Get<MongoDbSettings>();
                var mongoClient = new MongoClient(mongoDbSettings.ConnectionString);
                return mongoClient.GetDatabase(serviceSettings.ServiceName);
            });
            return services;
        }

        public static IServiceCollection AddMongoRepository<T>(
            this IServiceCollection services,
            string collectionName
        )
            where T : IEntity
        {
            services.AddSingleton<IRepository<T>>(ServiceProvider =>
            {
                var database = ServiceProvider.GetRequiredService<IMongoDatabase>();
                return new MongoRepository<T>(database, collectionName);
            });
            return services;
        }
    }
}
