using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Application.Declaration.Estimator;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;
using DeliveryDatePlanning.Domain.Enums;

namespace DeliveryDatePlanning.Application.Core.Estimator;

public class OverdueEstimator : IOverdueEstimator
{
    public Result<OverdueDays> Estimate(DateOnly now, Estimate current)
    {
        if (current.Status != EstimateStatusType.Expired)
        {
            return current.Overdue;
        }
        
        if (!current.DeliveryDates.DateEnd.HasValue)
        {
            return Result.Failure<OverdueDays>("Failed to calculate the number of days past due (current.DeliveryDates.DateEnd.HasValue == false)");
        }

        var overdue = now.DayNumber - current.DeliveryDates.DateEnd.Value.DayNumber;

        return OverdueDays.Create(overdue);

    }
}