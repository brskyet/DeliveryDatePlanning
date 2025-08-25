using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;

namespace DeliveryDatePlanning.UnitTest.Common.TestDataProviders;

public static class Estimates
{
    public static Estimate EstimateFactory(Invoice invoice, bool isPrevious, bool datesIsEmpty, bool datesHasBeenChanged)
    {
        var estimate = Estimate.CreateDefault(invoice).Value;
        
        if(isPrevious && datesIsEmpty == false)
        {
            estimate.SetDeliveryDates(
                DeliveryDates.Create(
                    DateOnly.FromDateTime(DateTime.Now.AddDays(-2)),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(-1))
                 
                ).Value);
        }  
        else if (isPrevious == false && datesHasBeenChanged)
        {
            estimate.SetDeliveryDates(
                DeliveryDates.Create(
                    DateOnly.FromDateTime(DateTime.Now),
                    DateOnly.FromDateTime(DateTime.Now.AddDays(2))
                ).Value);
        } 

        return estimate;
    }
}