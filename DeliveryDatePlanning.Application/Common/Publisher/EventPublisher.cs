using System.Text;
using Newtonsoft.Json;
using DeliveryDatePlanning.Application.Declaration.Publisher;
using RabbitMQ.Client;

namespace DeliveryDatePlanning.Application.Common.Publisher;

public class EventPublisher : IEventPublisher
{
    private IConnection RabbitConnection { get; }
    
    public EventPublisher(IConnection rabbitConnection)
    {
        this.RabbitConnection = rabbitConnection;
    }

    public async Task Publish(string exchangeName, object payload, CancellationToken token = default)
    {
        using var channel = this.RabbitConnection.CreateModel();
            
        channel.ExchangeDeclare(exchangeName, ExchangeType.Fanout, durable: true);

        var json = JsonConvert.SerializeObject(payload, new JsonSerializerSettings()
        {
            NullValueHandling = NullValueHandling.Ignore
        });

        var body = Encoding.UTF8.GetBytes(json);

        channel.BasicPublish(exchange: exchangeName,
            routingKey: "",
            basicProperties: null,
            body: body);
    }
}