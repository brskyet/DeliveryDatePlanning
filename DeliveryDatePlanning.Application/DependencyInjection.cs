using System.Reflection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using DeliveryDatePlanning.Application.Common.Constant;
using DeliveryDatePlanning.Application.Common.Mapping;
using DeliveryDatePlanning.Application.Common.Publisher;
using DeliveryDatePlanning.Application.Core.Estimator;
using DeliveryDatePlanning.Application.Core.Handler;
using DeliveryDatePlanning.Application.Core.Validator;
using DeliveryDatePlanning.Application.Declaration.Estimator;
using DeliveryDatePlanning.Application.Declaration.Handler;
using DeliveryDatePlanning.Application.Declaration.Notifier;
using DeliveryDatePlanning.Application.Declaration.Publisher;
using DeliveryDatePlanning.Application.Declaration.Validator;
using DeliveryDatePlanning.Domain.Core.Entity;
using Points.Protocol;
using TariffZones.Protocol;
using RabbitMQ.Client;

namespace DeliveryDatePlanning.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplicationWebApi(this IServiceCollection services)
    {
        services.AddAutoMapper(config =>
        {
            config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
        });

        services.AddScoped<IQueryValidator, QueryValidator>();
        services.AddScoped<IQueryHandler, QueryHandler>();

        return services;
    }


    public static IServiceCollection AddApplicationBackgroundService(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(config =>
        {
            config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
        });
        
        services.AddHttpClient<TariffZonesClient>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("Links:TariffZones"));
        });
        
        services.AddHttpClient<PointsClient>(client =>
        {
            client.BaseAddress = new Uri(configuration.GetValue<string>("Links:Points"));
        });
        
        services.Configure<ConnectionFactory>(configuration.GetSection("RabbitMq"));
            
        services.AddSingleton(sp =>
        {
            var cf = sp.GetRequiredService<IOptions<ConnectionFactory>>();
            return cf.Value.CreateConnection();
        });

        services.AddScoped<IEventPublisher, EventPublisher>();
        
        services.AddSingleton<IReadOnlyCollection<TariffZoneKey>, TariffZoneKeyCollection>();

        services.AddScoped<ICommandValidator, CommandValidator>();
        services.AddScoped<IEstimateValidator, EstimateValidator>();
        services.AddScoped<IEstimateResultNotifier, DummyEstimateResultNotifier>();
        services.AddScoped<ICommandHandler, CommandHandler>();
        
        services.AddScoped<IEstimationStrategyFactory, EstimationStrategyFactory>();

        return services;
    }
}