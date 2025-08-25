using MassTransitRMQExtensions.Attributes.ConsumerAttributes;
using DeliveryDatePlanning.Application.Common.Constant;
using DeliveryDatePlanning.Application.Declaration.Estimator;
using DeliveryDatePlanning.Application.Declaration.Handler;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Protocol.Models;
using Models;

namespace DeliveryDatePlanning.BackgroundService.Controllers;

public class DeliveryDateController
{
    private readonly ICommandHandler handler;
    private readonly IEstimationStrategyFactory factory;

    public DeliveryDateController(ICommandHandler handler, IEstimationStrategyFactory factory)
    {
        this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    [SubscribeBasicOn(ExchangeName.EncloseRegistered)]
    public async Task EncloseRegistered(IEnumerable<EventJson> events)
    {
        await this.HandleEnclose(ExchangeName.EncloseRegistered, events.ToArray(), this.factory.DateAndStatus());
    }
    
    [SubscribeBasicOn(ExchangeName.EncloseDeliveredToDeliveryPoint)]
    public async Task EncloseDeliveredToDeliveryPoint(IEnumerable<EventJson> events)
    {
        await this.HandleEnclose(ExchangeName.EncloseDeliveredToDeliveryPoint, events.ToArray(), this.factory.OnlyStatus());
    }
    
    [SubscribeBasicOn(ExchangeName.EncloseAcceptedInDeliveryPoint)]
    public async Task EncloseAcceptedInDeliveryPoint(IEnumerable<EventJson> events)
    {
        await this.HandleEnclose(ExchangeName.EncloseAcceptedInDeliveryPoint, events.ToArray(), this.factory.DateAndStatus());
    }
    
    [SubscribeBasicOn(ExchangeName.EncloseReceived)]
    public async Task EncloseReceived(IEnumerable<EventJson> events)
    {
        await this.HandleEnclose(ExchangeName.EncloseReceived, events.ToArray(), this.factory.OnlyStatus());
    }
    
    [SubscribeBasicOn(ExchangeName.EncloseReturnedToClient)]
    public async Task EncloseReturnedToClient(IEnumerable<EventJson> events)
    {
        await this.HandleEnclose(ExchangeName.EncloseReturnedToClient, events.ToArray(), this.factory.OnlyStatus());
    }
    
    [SubscribeBasicOn(ExchangeName.EncloseRejected)]
    public async Task EncloseRejected(IEnumerable<EventJson> events)
    {
        await this.HandleEnclose(ExchangeName.EncloseRejected, events.ToArray(), this.factory.OnlyStatus());
    }
    
    [SubscribeBasicOn(ExchangeName.EncloseReadyForReturn)]
    public async Task EncloseReadyForReturn(IEnumerable<EventJson> events)
    {
        await this.HandleEnclose(ExchangeName.EncloseReadyForReturn, events.ToArray(), this.factory.DateAndStatus());
    }
    
    [SubscribeBasicOn(ExchangeName.EncloseAnnulled)]
    public async Task EncloseAnnulled(IEnumerable<EventJson> events)
    {
        await this.HandleEnclose(ExchangeName.EncloseAnnulled, events.ToArray(), this.factory.OnlyStatus());
    }
    
    [SubscribeBasicOn(ExchangeName.InvoicePickupDateChanged)]
    public async Task InvoicePickupDateChanged(IEnumerable<EventJson> events)
    {
        await this.HandleInvoice(ExchangeName.InvoicePickupDateChanged, events.ToArray(), this.factory.DateAndStatus());
    }
    
    [SubscribeBasicOn(ExchangeName.InvoiceSenderCityChanged)]
    public async Task InvoiceSenderCityChanged(IEnumerable<EventJson> events)
    {
        await this.HandleInvoice(ExchangeName.InvoiceSenderCityChanged, events.ToArray(), this.factory.DateAndStatus());
    }
    
    [SubscribeBasicOn(ExchangeName.InvoiceRedirected)]
    public async Task InvoiceRedirected(IEnumerable<EventJson> events)
    {
        await this.HandleInvoice(ExchangeName.InvoiceRedirected, events.ToArray(), this.factory.DateAndStatus());
    }
    
    [SubscribeBasicOn(ExchangeName.InvoiceRecipientCityChanged)]
    public async Task InvoiceRecipientCityChanged(IEnumerable<EventJson> events)
    {
        await this.HandleInvoice(ExchangeName.InvoiceRecipientCityChanged, events.ToArray(), this.factory.DateAndStatus());
    }

    [SubscribeBasicOn(ExchangeName.InvoiceUtilization)]
    public async Task InvoiceUtilization(IEnumerable<EventJson> events)
    {
        await this.HandleInvoice(ExchangeName.InvoiceUtilization, events.ToArray(), this.factory.DateAndStatus());
    }

    [SubscribeBasicOn(ExchangeName.InvoiceSendToReturn)]
    public async Task InvoiceSendToReturn(IEnumerable<EventJson> events)
    {
        await this.HandleInvoice(ExchangeName.InvoiceSendToReturn, events.ToArray(), this.factory.DateAndStatus());
    }
    
    [SubscribeBasicOn(ExchangeName.InvoiceDeliveryModeChanged)]
    public async Task InvoiceDeliveryModeChanged(IEnumerable<EventJson> events)
    {
        await this.HandleInvoice(ExchangeName.InvoiceDeliveryModeChanged, events.ToArray(), this.factory.DateAndStatus());
    }

    private async Task HandleEnclose(string exchange, EventJson[] events, IEstimationStrategy strategy)
    {
        foreach (var @event in events)
        {
            await this.handler.Handle(new EncloseStateRecordCommand(@event.Id, @event.Time, (EncloseState) int.Parse(exchange)));
            
            await this.handler.Handle(new EstimateCommand(@event.Time, exchange).SetEnclose(@event.Id), strategy);
        }
    }

    private async Task HandleInvoice(string exchange, EventJson[] events, IEstimationStrategy strategy)
    {
        foreach (var @event in events)
        {
            await this.handler.Handle(new EstimateCommand(@event.Time, exchange).SetInvoice(@event.Id), strategy);
        }
    }
}