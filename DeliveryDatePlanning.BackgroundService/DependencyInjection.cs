using System.Reflection;
using MassTransitRMQExtensions;
using MassTransitRMQExtensions.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Winton.Extensions.Configuration.Consul;
using DeliveryDatePlanning.Application.Common.Mapping;
using DeliveryDatePlanning.Application.Common.Publisher;

namespace DeliveryDatePlanning.BackgroundService;

public static class DependencyInjection
{
    public static IServiceCollection AddMapping(this IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
        });
        
        return services;
    }

    public static IServiceCollection AddMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var mqConfig = configuration.GetSection("RabbitMq").Get<RabbitSettings>();

        services.ConfigureMassTransitControllers(new RabbitMqConfig(mqConfig.UserName, mqConfig.Password,
            new Uri($"amqp://{mqConfig.HostName}:{mqConfig.Port}")));

        return services;
    }
    
    public static IConfigurationBuilder AddConsul(this IConfigurationBuilder builder)
    {
        var confRoot = builder.Build();
        var consulConfig = confRoot.GetSection("Consul").Get<ConsulOptions>();
        var token = confRoot.GetValue<string>("consulToken");
        
        return builder.AddConsul(consulConfig.Key,
            o =>
            {
                o.ConsulConfigurationOptions = cco =>
                {
                    cco.Address = new Uri(consulConfig.Host);
                    cco.Token = token;
                };
                o.ReloadOnChange = true;
            });
    }
    
    public class ConsulOptions
    {
        public string Key { get; set; }

        public string Host { get; set; }
    }
}