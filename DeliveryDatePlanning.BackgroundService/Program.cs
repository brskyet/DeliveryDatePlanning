using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;
using NLog.Web;

namespace DeliveryDatePlanning.BackgroundService;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = new HostBuilder()
            .ConfigureHostConfiguration(config =>
            {
                config.AddCommandLine(args);
                config.AddEnvironmentVariables();
            })
            .ConfigureAppConfiguration((context, config) =>
            {
                var env = context.HostingEnvironment.EnvironmentName;

                config.AddJsonFile("appsettings.BackgroundService.json");
                config.AddJsonFile($"appsettings.BackgroundService.{env}.json", optional: true, reloadOnChange: true);
            })
            .ConfigureServices((hostContext, services) =>
            {
                var startup = new Startup(hostContext.Configuration);
                startup.ConfigureServices(services);
            })
            .ConfigureLogging((context, logging) =>
            {
                logging.ClearProviders();
                logging.AddNLogWeb(new NLogLoggingConfiguration(context.Configuration.GetSection("NLog")));
            })
            .UseNLog()
            .ConfigureAppConfiguration(config => config.AddConsul());

        await builder.RunConsoleAsync();
    }
}