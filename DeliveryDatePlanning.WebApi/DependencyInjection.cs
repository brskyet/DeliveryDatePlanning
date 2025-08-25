using Winton.Extensions.Configuration.Consul;

namespace DeliveryDatePlanning.WebApi;

public static class DependencyInjection
{
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