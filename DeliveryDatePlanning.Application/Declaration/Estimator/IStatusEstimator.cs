using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;

namespace DeliveryDatePlanning.Application.Declaration.Estimator;

public interface IStatusEstimator
{
    Result<EstimateStatusTypeEnum> Estimate(DateOnly now, Estimate current);
}