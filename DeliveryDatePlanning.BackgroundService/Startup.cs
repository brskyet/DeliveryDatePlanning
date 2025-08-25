using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using DeliveryDatePlanning.Application;
using DeliveryDatePlanning.Data;

namespace DeliveryDatePlanning.BackgroundService;

public class Startup
{
    private IConfiguration Configuration { get; }
        
    public Startup(IConfiguration configuration)
    {
        this.Configuration = configuration;
    }

    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationBackgroundService(Configuration);
        
        services.AddMapping();
        
        services.AddData(Configuration);
        
        services.AddHostedService<IndexHostedService>();
        
        services.AddMassTransit(Configuration);
    }
}