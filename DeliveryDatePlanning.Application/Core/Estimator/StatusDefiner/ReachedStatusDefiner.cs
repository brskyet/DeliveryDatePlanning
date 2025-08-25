using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Enums;
using Models;

namespace DeliveryDatePlanning.Application.Core.Estimator.StatusDefiner;

public class ReachedStatusDefiner : StatusDefinerChain
{
    private readonly IReadOnlyCollection<EncloseState> states = new[]
    {
        EncloseState.AcceptedByRegistry, EncloseState.Registered, EncloseState.FormedForPassingToLogistician, EncloseState.AcceptedByLogistician, 
        EncloseState.OnLogisticianKladovka, EncloseState.OnRoute, EncloseState.OnCourier, EncloseState.PassedForOutOfCityDelivery, 
        EncloseState.AcceptedForOutOfCityDelivery, EncloseState.BaledForOutOfCityDelivery, EncloseState.CourierAssigned, EncloseState.Consolidated, 
        EncloseState.Searched, EncloseState.Lost, EncloseState.ScannedForDelivery, EncloseState.ScannedToConsolidate, EncloseState.ScannedToRegistry, 
        EncloseState.PassedFictionally, EncloseState.PassedToInIntraregionalRoute, EncloseState.OnCourierConsolidated,
    };
    
    public override EstimateStatusType DefineStatus(DateOnly now, Estimate current)
    {
        if (current.IsDeliveryDateEmpty() == false &&
            current.Status == EstimateStatusType.NotReached &&
            current.DeliveryDates.DateStart <= now && 
            current.DeliveryDates.DateEnd >= now && 
            current.Invoice.AllEnclosesInSameState(this.states))
        {
            return EstimateStatusType.Reached;
        }
        
        if (this.Successor is not null)
        {
            return this.Successor.DefineStatus(now, current);
        }

        return current.Status;
    }
}