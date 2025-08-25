namespace DeliveryDatePlanning.Application.Declaration.Publisher;

public interface IEventPublisher
{
    Task Publish(string exchangeName, object payload, CancellationToken token = default);
}