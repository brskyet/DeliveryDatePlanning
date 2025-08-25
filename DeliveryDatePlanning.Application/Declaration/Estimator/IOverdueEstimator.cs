using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;

namespace DeliveryDatePlanning.Application.Declaration.Estimator;

public interface IOverdueEstimator
{
    Result<OverdueDays> Estimate(DateOnly now, Estimate current);
}