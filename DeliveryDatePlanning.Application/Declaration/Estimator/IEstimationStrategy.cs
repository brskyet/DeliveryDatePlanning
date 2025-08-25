using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Domain.Core.Entity;

namespace DeliveryDatePlanning.Application.Declaration.Estimator;

public interface IEstimationStrategy
{
    Task<Result<Estimate>> Estimate(EstimateCommand command, Invoice invoice, Estimate current, CancellationToken token = default);
}