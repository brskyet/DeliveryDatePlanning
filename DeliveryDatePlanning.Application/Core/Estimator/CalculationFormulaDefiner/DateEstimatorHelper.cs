using Points.Protocol.Models;

namespace DeliveryDatePlanning.Application.Core.Estimator.CalculationFormulaDefiner;

public static class DateEstimatorHelper
{
    public static DateOnly AddDaysUntilFirstServiceDay(this DateOnly date, Point point)
    {
        if (point is null)
        {
            return date;
        }
        
        while ((point.ServiceDateExceptions != null && point.ServiceDateExceptions.Any(edt => Equals(date, DateOnly.FromDateTime(edt.Date)))
               || point.ServiceSchedule.Days.All(d => d.Key != date.DayOfWeek))
               && (point.ServiceDatePositiveExceptions == null
                   || !point.ServiceDatePositiveExceptions.Any(edt => Equals(date, DateOnly.FromDateTime(edt.Date)))))
        {
            date = date.AddDays(1);
        }

        return date;
    }
}