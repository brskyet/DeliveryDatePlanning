using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Enums;
using Models;

namespace DeliveryDatePlanning.Application.Core.Estimator.StatusDefiner;

public class NotReachedRejectedStatusDefiner : StatusDefinerChain
{
    private readonly IReadOnlyCollection<EncloseState> states = new[]
    {
        EncloseState.Rejected, EncloseState.Annulled
    };

    public override EstimateStatusType DefineStatus(DateOnly now, Estimate current)
    {
        if (ShouldSetNotReachedRejected(now, current))
        {
            return EstimateStatusType.NotReachedRejected;
        }

        if (this.Successor is not null)
        {
            return this.Successor.DefineStatus(now, current);
        }

        return current.Status;
    }

    private bool ShouldSetNotReachedRejected(DateOnly now, Estimate current)
    {
        var isNotReached = current.Status == EstimateStatusType.NotReached;
        var isInvoiceEnclosesRejectedOrAnnulled = current.Invoice.AllEnclosesInSameState(this.states);
        var invoiceHasSpecialFlags = current.Invoice.IsUtilization || current.Invoice.IsSendToReturn;

        return
            isNotReached
            && (isInvoiceEnclosesRejectedOrAnnulled || invoiceHasSpecialFlags);
            
    }
}