using MassTransitRMQExtensions.Attributes.ConsumerAttributes;
using MassTransitRMQExtensions.Models;
using DeliveryDatePlanning.Application.Common.Constant;
using DeliveryDatePlanning.Application.Declaration.Handler;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Protocol.Models;

namespace DeliveryDatePlanning.BackgroundService.Controllers;

public class UpdateInvoiceDataController 
{
    private readonly ICommandHandler handler;

    public UpdateInvoiceDataController(ICommandHandler handler)
    {
        this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
    }
    
    [SubscribeBasicOn(ExchangeName.InvoiceRedirected)]
    public async Task InvoiceDataChanged(MsgContext<IEnumerable<EventJson>> events)
    {
        var exchange = events.Exchange;
        
        foreach (var @event in events.Message)
        {
            await this.handler.Handle(new InvoiceDataChangedCommand(@event.Id, exchange));
        }
    }
}