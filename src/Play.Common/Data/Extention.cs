using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Play.Common.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;

namespace Play.Common.Data;

public static class Extention
{
    public static IServiceCollection AddMongo(this IServiceCollection services, IConfiguration configuration)
    {
        BsonSerializer.RegisterSerializer(new GuidSerializer(BsonType.String));
        BsonSerializer.RegisterSerializer(new DateTimeSerializer(BsonType.DateTime));

        // Add Settings, Env Variables, 
        //var serviceSettings = configuration.GetSection(nameof(ServiceSettings))
        //    .Get<ServiceSettings>();

        var mongoDbSettings = configuration.GetSection(nameof(MongoDbSettings))
            .Get<MongoDbSettings>();

        if (null == mongoDbSettings)
            throw new ArgumentNullException(nameof(mongoDbSettings), "Mongo Db Settings NOT Found in Config.");

        services.AddSingleton(serviceProvider =>
        {
            var mangoClient = new MongoClient(mongoDbSettings.ConnectionString);
            return mangoClient.GetDatabase(mongoDbSettings.DatabaseName);
        });

        return services;
    }

    public static IServiceCollection AddMongoRepo<T>(this IServiceCollection services) where T : IMustHaveId
    {
        services.AddSingleton<IRepo<T>, GenericRepository<T>>();

        return services;
    }

    public static IServiceCollection AddMongoRepo<TEntity, TRepo>(this IServiceCollection services)
        where TEntity : IMustHaveId 
        where TRepo : GenericRepository<TEntity>
    {
        services.AddSingleton<IRepo<TEntity>, TRepo>();

        return services;
    }
}
