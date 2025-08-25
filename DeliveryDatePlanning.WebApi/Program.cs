using NLog;
using NLog.Extensions.Logging;

namespace DeliveryDatePlanning.WebApi;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .Build();

        LogManager.Configuration = new NLogLoggingConfiguration(config.GetSection("NLog"));
        
        var logger = NLog.Web.NLogBuilder.ConfigureNLog(LogManager.Configuration).GetCurrentClassLogger();
        
        try
        {
            logger.Info("DeliveryDatePlanning.WebApi launched");
            
            var host = CreateHostBuilder(args).Build();

            await host.RunAsync();
        }
        catch (Exception exception) 
        {
            logger.Fatal($"Unhandled error. Message - {exception.Message}. InnerException message = {exception.InnerException?.Message ?? "null"}");
        }
        finally
        {
            logger.Info("DeliveryDatePlanning.WebApi stopped");
        }
    }

    private static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddNLog(new NLogLoggingConfiguration(context.Configuration.GetSection("NLog")));
            })
            .ConfigureWebHostDefaults(webBuilder => { webBuilder.UseStartup<Startup>(); })
            .ConfigureAppConfiguration(config => config.AddConsul());
}