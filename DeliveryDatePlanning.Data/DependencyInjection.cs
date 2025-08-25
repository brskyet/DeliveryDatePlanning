using System.Reflection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using DeliveryDatePlanning.Application.Common.Mapping;
using DeliveryDatePlanning.Application.Declaration.Store;
using DeliveryDatePlanning.Data.Context;
using DeliveryDatePlanning.Data.Index;
using DeliveryDatePlanning.Data.Store;

namespace DeliveryDatePlanning.Data;

public static class DependencyInjection
{
    public static IServiceCollection AddData(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddAutoMapper(config =>
        {
            config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
        });
        
        var connectionString = configuration.GetConnectionString("Apt");

        services.AddDbContext<AptContext>(options =>
        {
            options.UseSqlServer(connectionString, opt =>
            {
                opt.MigrationsAssembly("DeliveryDatePlanning.Data");
                opt.MigrationsHistoryTable("__DeliveryDatePlanningMigrations");
            });
        });
        
        using (ServiceProvider serviceProvider = services.BuildServiceProvider())
        {
            using var aptContext = serviceProvider.GetRequiredService<AptContext>();
            aptContext.Database.Migrate();
        }
        
        var mongodbConnectionString = configuration.GetConnectionString("Mongo");
        
        services.AddSingleton<IMongoClient, MongoClient>(sp => new MongoClient(mongodbConnectionString));
        
        services.AddSingleton(sp =>
        {
            var mongoUrl = new MongoUrl(mongodbConnectionString);
            
            var client = sp.GetRequiredService<IMongoClient>();
            
            return client.GetDatabase(mongoUrl.DatabaseName);
        });

        services.AddScoped<EstimateDbContext>();
        
        services.AddScoped<IInvoiceStore, InvoiceStore>();
        services.AddScoped<IEstimateStore, EstimateStore>();
        services.AddScoped<ITariffZoneKeyStore, TariffZoneKeyStore>();
        services.AddScoped<ICityStore, CityStore>();
        
        services.AddScoped<MongoDbIndex>();

        return services;
    }
}