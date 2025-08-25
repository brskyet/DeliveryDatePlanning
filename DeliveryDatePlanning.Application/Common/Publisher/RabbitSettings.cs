namespace DeliveryDatePlanning.Application.Common.Publisher;

public class RabbitSettings
{
    public string HostName { get; set; }
    public int Port { get; set; }
    public string UserName { get; set; }
    public string Password { get; set; }
}