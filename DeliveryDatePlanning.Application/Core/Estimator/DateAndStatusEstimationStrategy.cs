using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Application.Declaration.Estimator;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Domain.Core.Entity;

namespace DeliveryDatePlanning.Application.Core.Estimator;

public class DateAndStatusEstimationStrategy : IEstimationStrategy
{
    private readonly IDateEstimator dateEstimator;
    private readonly IStatusEstimator statusEstimator;
    private readonly IOverdueEstimator overdueEstimator;
    private readonly IDateChangeReasonEstimator dateChangeReasonEstimator;

    public DateAndStatusEstimationStrategy(IDateEstimator dateEstimator, IStatusEstimator statusEstimator, IOverdueEstimator overdueEstimator, IDateChangeReasonEstimator dateChangeReasonEstimator)
    {
        this.dateEstimator = dateEstimator ?? throw new ArgumentNullException(nameof(dateEstimator));
        this.statusEstimator = statusEstimator ?? throw new ArgumentNullException(nameof(statusEstimator));
        this.overdueEstimator = overdueEstimator ?? throw new ArgumentNullException(nameof(overdueEstimator));
        this.dateChangeReasonEstimator = dateChangeReasonEstimator ?? throw new ArgumentNullException(nameof(dateChangeReasonEstimator));
    }

    public async Task<Result<Estimate>> Estimate(EstimateCommand command, Invoice invoice, Estimate current, CancellationToken token = default)
    {
        var result = current == default 
            ? Domain.Core.Entity.Estimate.CreateDefault(invoice).Value 
            : Domain.Core.Entity.Estimate.Create(current).Value;

        var dateEstimationResult = await this.dateEstimator.Estimate(invoice, result, command, token);

        if (dateEstimationResult.IsFailure)
        {
            return Result.Failure<Estimate>(dateEstimationResult.Error);
        }

        result.SetDeliveryDates(dateEstimationResult.Value);
        
        var dateChangeReasonResult = this.dateChangeReasonEstimator.Estimate(command, invoice, result, current);

        if (dateChangeReasonResult.IsFailure)
        {
            return Result.Failure<Estimate>(dateChangeReasonResult.Error);
        }

        result.SetDateChangeReason(dateChangeReasonResult.Value);
        
        var statusResult = this.statusEstimator.Estimate(DateOnly.FromDateTime(command.Timestamp), result);
        
        if (statusResult.IsFailure)
        {
            return Result.Failure<Estimate>(statusResult.Error);
        }

        result.SetStatus(statusResult.Value);
        
        var overdueResult = this.overdueEstimator.Estimate(DateOnly.FromDateTime(command.Timestamp), result);
            
        if (overdueResult.IsFailure)
        {
            return Result.Failure<Estimate>(overdueResult.Error);
        }

        result.SetOverdue(overdueResult.Value);

        if (command.IsInvoiceDataChanged)
        {
            result.SetInvoice(invoice);
        }

        return result;
    }
}