using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;

namespace DeliveryDatePlanning.Application.Declaration.Estimator;

public interface IDateEstimator
{
    Task<Result<DeliveryDates>> Estimate(Invoice invoice, Estimate current, EstimateCommand command, CancellationToken token = default);
}