using MongoDB.Bson.Serialization.Serializers;
using MongoDB.Bson.Serialization;
using MongoDB.Bson;
using MongoDB.Driver;
using Play.Common.Config;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Configuration;
using MassTransit;
using System.Reflection;

namespace Play.Common.MassTransit;

public static class Extention
{
    public static IServiceCollection AddMassTransitWithRabbitMQ(this IServiceCollection services, IConfiguration configuration)
    {
        // Add Message Brocker
        _ = services.AddMassTransit(configure =>
        {
            configure.AddConsumers(Assembly.GetEntryAssembly());

            configure.UsingRabbitMq((context, configurator) =>
            {
                //var configuration = context.GetService<IConfiguration>();
                var serviceSettings = configuration.GetSection(nameof(ServiceSettings))
                    .Get<ServiceSettings>();

                _ = serviceSettings ?? throw new ArgumentNullException(nameof(serviceSettings));

                var rabbitMQSettings = configuration.GetSection(nameof(RabbitMQSettings))
                    .Get<RabbitMQSettings>();

                _ = rabbitMQSettings ?? throw new ArgumentNullException(nameof(serviceSettings));

                configurator.Host(rabbitMQSettings.Host);
                configurator.ConfigureEndpoints(context, new KebabCaseEndpointNameFormatter(serviceSettings.ServiceName, false));
                
                configurator.UseMessageRetry(retryConfigurator => 
                {
                    retryConfigurator.Interval(3, TimeSpan.FromSeconds(10));
                });
            });
        });

        return services;
    }
}
