using System.Reflection;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Converters;
using DeliveryDatePlanning.Application;
using DeliveryDatePlanning.Application.Common.Mapping;
using DeliveryDatePlanning.Data;
using DeliveryDatePlanning.WebApi.Middleware;
using DeliveryDatePlanning.WebApi.Schema;

namespace DeliveryDatePlanning.WebApi;

public class Startup
{
    private IConfiguration Configuration { get; }
    
    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }
    
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddApplicationWebApi();

        services.AddData(Configuration);
        
        services.AddAutoMapper(config =>
        {
            config.AddProfile(new AssemblyMappingProfile(Assembly.GetExecutingAssembly()));
        });
        
        services.AddControllers().AddNewtonsoftJson(o =>
        {
            o.SerializerSettings.Converters.Add(new StringEnumConverter());
        });
        
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo {Title = "DeliveryDatePlanning.WebApi", Version = "v1"});
            
            c.UseInlineDefinitionsForEnums();

            var filePath = Path.Combine(AppContext.BaseDirectory, $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
            
            c.IncludeXmlComments(filePath);
            
            c.SchemaFilter<EnumTypesSchemaFilter>(filePath);
            
            filePath = Path.Combine(AppContext.BaseDirectory, "DeliveryDatePlanning.Domain.xml");
            
            c.IncludeXmlComments(filePath);

            c.SchemaFilter<EnumTypesSchemaFilter>(filePath);
            
        }).AddSwaggerGenNewtonsoftSupport();
    }

    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        
        app.UseRouting();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

        app.UseSwagger();
        
        app.UseSwaggerUI(c =>
        {
            c.SwaggerEndpoint("/swagger/v1/swagger.json", "DeliveryDatePlanning.WebApi - v1");
        });
    }
}