using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;


namespace DeliveryDatePlanning.Application.Declaration.Estimator;

public interface IDateChangeReasonEstimator
{
    Result<DateChangeReasonTypeEnum> Estimate(EstimateCommand command, Invoice invoice, Estimate current, Estimate previousEstimate);
}