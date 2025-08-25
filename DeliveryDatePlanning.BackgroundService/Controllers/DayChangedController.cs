using MassTransitRMQExtensions.Attributes.ConsumerAttributes;
using MassTransitRMQExtensions.Attributes.JobAttributes;
using DeliveryDatePlanning.Application.Common.Constant;
using DeliveryDatePlanning.Application.Declaration.Estimator;
using DeliveryDatePlanning.Application.Declaration.Handler;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Application.Declaration.Publisher;

namespace DeliveryDatePlanning.BackgroundService.Controllers;

public class DayChangedController
{
    private readonly ICommandHandler handler;
    private readonly IEstimationStrategyFactory factory;

    public DayChangedController(ICommandHandler handler, IEstimationStrategyFactory factory)
    {
        this.handler = handler ?? throw new ArgumentNullException(nameof(handler));
        this.factory = factory ?? throw new ArgumentNullException(nameof(factory));
    }

    [RunJob("0 15 0/1 ? * * *")] // at minute 15 every hour
    public async Task DayChangedPublishing()
    {
        // т.к. все события и параметры инфраструктуры и окружения фиксируются относительно Москвы
        var moscowZone = TimeZoneInfo.FindSystemTimeZoneById("Russian Standard Time");
        var moscowTime = TimeZoneInfo.ConvertTime(DateTime.UtcNow, moscowZone);
        
        await this.handler.Handle(new DayChangedPublishingCommand(moscowTime));
    }

    [SubscribeBasicOn(ExchangeName.DayChangedForStatusEstimation)]
    public async Task EstimateInvoiceStatus(DayChangedEstimationEvent payload)
    {
        await this.handler.Handle(new EstimateCommand(payload.Timestamp, ExchangeName.DayChangedForStatusEstimation)
            .SetInvoice(payload.InvoiceKey), this.factory.OnlyStatus());
    }
    
    [SubscribeBasicOn(ExchangeName.DayChangedForOverdueEstimation)]
    public async Task EstimateInvoiceOverdue(DayChangedEstimationEvent payload)
    {
        await this.handler.Handle(new EstimateCommand(payload.Timestamp, ExchangeName.DayChangedForOverdueEstimation)
            .SetInvoice(payload.InvoiceKey), this.factory.OnlyStatus());
    }
    
    [SubscribeBasicOn(ExchangeName.DayChangedForNotAcceptedInvoiceEstimation)]
    public async Task EstimateNotAcceptedInvoice(DayChangedEstimationEvent payload)
    {
        await this.handler.Handle(new EstimateCommand(payload.Timestamp, ExchangeName.DayChangedForNotAcceptedInvoiceEstimation)
            .SetInvoice(payload.InvoiceKey), this.factory.DateAndStatus());
    }
}