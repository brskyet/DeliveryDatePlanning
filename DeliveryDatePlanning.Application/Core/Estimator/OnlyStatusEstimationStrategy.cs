using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Application.Declaration.Estimator;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Domain.Core.Entity;

namespace DeliveryDatePlanning.Application.Core.Estimator;

public class OnlyStatusEstimationStrategy : IEstimationStrategy
{
    private readonly IStatusEstimator statusEstimator;
    private readonly IOverdueEstimator overdueEstimator;

    public OnlyStatusEstimationStrategy(IStatusEstimator statusEstimator, IOverdueEstimator overdueEstimator)
    {
        this.statusEstimator = statusEstimator ?? throw new ArgumentNullException(nameof(statusEstimator));
        this.overdueEstimator = overdueEstimator ?? throw new ArgumentNullException(nameof(overdueEstimator));
    }

    public Task<Result<Estimate>> Estimate(EstimateCommand command, Invoice invoice, Estimate current, CancellationToken token = default)
    {
        var result = Domain.Core.Entity.Estimate.Create(current);
        
        var statusResult = this.statusEstimator.Estimate(DateOnly.FromDateTime(command.Timestamp), current);

        if (statusResult.IsFailure)
        {
            return Task.FromResult(Result.Failure<Estimate>(statusResult.Error));
        }

        result.Value.SetStatus(statusResult.Value);

        var overdueResult = this.overdueEstimator.Estimate(DateOnly.FromDateTime(command.Timestamp), result.Value);
            
        if (overdueResult.IsFailure)
        {
            return Task.FromResult(Result.Failure<Estimate>(overdueResult.Error));
        }

        result.Value.SetOverdue(overdueResult.Value);
        
        return Task.FromResult(result);
    }
}