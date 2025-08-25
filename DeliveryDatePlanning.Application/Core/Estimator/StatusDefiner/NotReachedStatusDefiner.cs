using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Enums;
using Models;

namespace DeliveryDatePlanning.Application.Core.Estimator.StatusDefiner;

public class NotReachedStatusDefiner : StatusDefinerChain
{
    private readonly IReadOnlyCollection<EncloseState> states = new[]
    {
        EncloseState.Rejected, EncloseState.Annulled, EncloseState.DeliveredToPT, EncloseState.ReturnedToClient, EncloseState.Received
    };

    public override EstimateStatusType DefineStatus(DateOnly now, Estimate current)
    {
        if(ShouldSetNotReached(now, current))
        {
            return EstimateStatusType.NotReached;
        }
        
        if (this.Successor is not null)
        {
            return this.Successor.DefineStatus(now, current);
        }

        return current.Status;
    }

    private bool ShouldSetNotReached(DateOnly now, Estimate current)
    {
        var hasDeliveryDate = current.IsDeliveryDateEmpty() == false;
        var deliveryDateIsGreaterThenToday = current.DeliveryDates.DateStart > now;
        var enclosesIsNotInForbiddenStates = current.Invoice.AllEnclosesInSameState(this.states) == false;
        var invoiceHasNoSpecialFlags = !current.Invoice.IsUtilization && !current.Invoice.IsSendToReturn;

        return
            hasDeliveryDate
            && deliveryDateIsGreaterThenToday
            && enclosesIsNotInForbiddenStates 
            && invoiceHasNoSpecialFlags;
    }
}