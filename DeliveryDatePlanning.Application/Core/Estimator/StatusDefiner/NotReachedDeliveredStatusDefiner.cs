using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Enums;
using Models;

namespace DeliveryDatePlanning.Application.Core.Estimator.StatusDefiner;

public class NotReachedDeliveredStatusDefiner : StatusDefinerChain
{
    private readonly IReadOnlyCollection<EncloseState> states = new[]
    {
        EncloseState.DeliveredToPT, EncloseState.Received, EncloseState.ReturnedToClient
    };
    
    public override EstimateStatusType DefineStatus(DateOnly now, Estimate current)
    {
        if (current.Status == EstimateStatusType.NotReached &&
            current.Invoice.AllEnclosesInSameState(this.states))
        {
            return EstimateStatusType.NotReachedDelivered;
        }
        
        if (this.Successor is not null)
        {
            return this.Successor.DefineStatus(now, current);
        }

        return current.Status;
    }
}