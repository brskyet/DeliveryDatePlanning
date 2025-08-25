using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Application.Core.Estimator.StatusDefiner;
using DeliveryDatePlanning.Application.Declaration.Estimator;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;

namespace DeliveryDatePlanning.Application.Core.Estimator;

public class StatusEstimator : IStatusEstimator
{
    private readonly StatusDefinerChain chain;
    
    public StatusEstimator()
    {
        chain = new NotReachedStatusDefiner()
            .SetSuccessor(new NotReachedDeliveredStatusDefiner()
                .SetSuccessor(new NotReachedRejectedStatusDefiner()
                    .SetSuccessor(new ReachedStatusDefiner()
                        .SetSuccessor(new ExpiredStatusDefiner()))));
    }
    
    public Result<EstimateStatusTypeEnum> Estimate(DateOnly now, Estimate current)
    {
        if (current == null)
        {
            return Result.Failure<EstimateStatusTypeEnum>($"[{nameof(StatusEstimator)}] {nameof(current)} estimate entity must be created");
        }

        if (current.IsDeliveryDateEmpty())
        {
            return current.Status;
        }
        
        var status = chain.DefineStatus(now, current);

        return EstimateStatusTypeEnum.Create(status);
    }
}