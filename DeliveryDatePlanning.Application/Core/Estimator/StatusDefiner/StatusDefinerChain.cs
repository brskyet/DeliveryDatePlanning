using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Enums;

namespace DeliveryDatePlanning.Application.Core.Estimator.StatusDefiner;

public abstract class StatusDefinerChain
{
    protected StatusDefinerChain Successor;
    
    public StatusDefinerChain SetSuccessor(StatusDefinerChain successor)
    {
        this.Successor = successor;

        return this;
    }
    
    public abstract EstimateStatusType DefineStatus(DateOnly now, Estimate current);
}