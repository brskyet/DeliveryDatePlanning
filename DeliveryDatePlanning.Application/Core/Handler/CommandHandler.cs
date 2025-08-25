using System.Text.Json;
using Microsoft.Extensions.Logging;
using DeliveryDatePlanning.Application.Common.Constant;
using DeliveryDatePlanning.Application.Declaration.Estimator;
using DeliveryDatePlanning.Application.Declaration.Handler;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Application.Declaration.Notifier;
using DeliveryDatePlanning.Application.Declaration.Publisher;
using DeliveryDatePlanning.Application.Declaration.Store;
using DeliveryDatePlanning.Application.Declaration.Validator;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;

namespace DeliveryDatePlanning.Application.Core.Handler;

public class CommandHandler : ICommandHandler
{
    private readonly IInvoiceStore invoiceStore;
    private readonly ICommandValidator commandValidator;
    private readonly IEstimateValidator estimateValidator;
    private readonly IEstimateStore estimateStore;
    private readonly IEstimateResultNotifier estimateNotifier;
    private readonly IEventPublisher eventPublisher;
    private readonly ICityStore cityStore;
    private readonly ILogger<CommandHandler> logger;

    public CommandHandler(IInvoiceStore invoiceStore, ICommandValidator commandValidator, IEstimateValidator estimateValidator, 
        IEstimateStore estimateStore, IEstimateResultNotifier estimateNotifier, IEventPublisher eventPublisher, ICityStore cityStore, 
        ILogger<CommandHandler> logger)
    {
        this.invoiceStore = invoiceStore ?? throw new ArgumentNullException(nameof(invoiceStore));
        this.commandValidator = commandValidator ?? throw new ArgumentNullException(nameof(commandValidator));
        this.estimateValidator = estimateValidator ?? throw new ArgumentNullException(nameof(estimateValidator));
        this.estimateStore = estimateStore ?? throw new ArgumentNullException(nameof(estimateStore));
        this.estimateNotifier = estimateNotifier ?? throw new ArgumentNullException(nameof(estimateNotifier));
        this.eventPublisher = eventPublisher ?? throw new ArgumentNullException(nameof(eventPublisher));
        this.cityStore = cityStore ?? throw new ArgumentNullException(nameof(cityStore));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task Handle(EstimateCommand command, IEstimationStrategy strategy, CancellationToken token)
    {
        var estimate = command.ShouldProcessLinkedInvoice 
            ? default 
            : await this.estimateStore.FindOrDefault(new EstimateFilter(command.InvoiceKey, command.EncloseKey), token);

        var invoice = await this.GetInvoiceAsync(estimate, command, token);

        if (this.commandValidator.IsValid(command, invoice, estimate) == false)
        {
            this.logger.LogInformation("{Record}", JsonSerializer.Serialize(new { message = "Command is not valid", command }));
            
            return;
        }
        
        if (command.IsInvoiceReceptionDateChanged && estimate is not null)
        {
            estimate.Invoice.SetReceptionDate(invoice.ReceptionDate);
            
            await this.estimateStore.UpdateReceptionDate(estimate, token);
        }

        var result = await strategy.Estimate(command, invoice, estimate, token);

        if (this.estimateValidator.IsValid(estimate, result) == false)
        {
            this.logger.LogInformation("{Record}", JsonSerializer.Serialize(new { message = "Estimate result is the same or not valid", command }));
            
            return;
        }

        await this.estimateStore.Upsert(result.Value, token);

        await this.estimateNotifier.Notify(result.Value, token);
    }

    public async Task Handle(EncloseStateRecordCommand command, CancellationToken token = default)
    {
        var estimate = await this.estimateStore.FindOrDefault(EstimateFilter.ByEncloseKey(command.Key), token);
        
        if (this.commandValidator.IsValid(command, estimate) == false)
        {
            this.logger.LogInformation("{Record}", JsonSerializer.Serialize(new { message = "Command is not valid", command }));
            
            return;
        }

        if (estimate.Invoice.Encloses.Any(e => e.Id == command.Key 
            && e.StateHistory.Any(sh => sh.State == command.State && sh.Timestamp == command.Timestamp)))
        {
            return;
        }
        
        estimate.AddEncloseStateRecord(command.Key, command.Timestamp, command.State);
        
        await this.estimateStore.UpdateEncloseStateRecord(estimate, token);
    }

    public async Task Handle(DayChangedPublishingCommand command, CancellationToken token = default)
    {
        var cities = await this.cityStore.FindMidnightCities(command.Timestamp, token);

        this.logger.LogInformation("{Record}", JsonSerializer.Serialize(new { message = $"Midnight cities: {string.Join("; ", cities.Select(c => c.ToString()))}" }));

        foreach (var city in cities)
        {
            var timestamp = command.Timestamp.AddHours(city.TimeZone);
                
            var notAccepted = await this.estimateStore.RelevantDayChangedForNotAccepted(city.Id, token: token);

            foreach (var invoiceKey in notAccepted)
            {
                await this.eventPublisher.Publish(ExchangeName.DayChangedForNotAcceptedInvoiceEstimation, new DayChangedEstimationEvent(invoiceKey, timestamp), token);
            }
            
            var overdue = await this.estimateStore.RelevantDayChangedForOverdue(city.Id, token: token);

            foreach (var invoiceKey in overdue)
            {
                await this.eventPublisher.Publish(ExchangeName.DayChangedForOverdueEstimation, new DayChangedEstimationEvent(invoiceKey, timestamp), token);
            }
            
            var statusChange = await this.estimateStore.RelevantDayChangedForStatus(city.Id, token: token);

            foreach (var invoiceKey in statusChange)
            {
                await this.eventPublisher.Publish(ExchangeName.DayChangedForStatusEstimation, new DayChangedEstimationEvent(invoiceKey, timestamp), token);
            }
        }
    }

    public async Task Handle(InvoiceDataChangedCommand command, CancellationToken token = default)
    {
        var estimate = await this.estimateStore.FindOrDefault(EstimateFilter.ByInvoiceKey(command.Key), token);

        var invoice = await this.invoiceStore.Find(InvoiceFilter.ByInvoiceKey(command.Key), token);
        
        if (this.commandValidator.IsValid(command, invoice, estimate) == false)
        {
            this.logger.LogInformation("{Record}", JsonSerializer.Serialize(new { message = "Command is not valid", command }));
            
            return;
        }
        
        await this.estimateStore.UpdateInvoice(EstimateFilter.ByInvoiceKey(command.Key), invoice, token);
    }

    private async Task<Invoice> GetInvoiceAsync(Estimate estimate, EstimateCommand command, CancellationToken token)
    {
        if (estimate is not null && !command.IsInvoiceDataChanged)
            return estimate.Invoice;
        
        if (command.ShouldProcessLinkedInvoice) 
            return await this.invoiceStore.FindLinked(new InvoiceFilter(command.InvoiceKey, command.EncloseKey), token);
        
        return await this.invoiceStore.Find(new InvoiceFilter(command.InvoiceKey, command.EncloseKey), token);
    }
}