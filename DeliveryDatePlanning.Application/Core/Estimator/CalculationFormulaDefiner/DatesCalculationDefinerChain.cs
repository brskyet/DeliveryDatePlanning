using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;
using DeliveryDatePlanning.Domain.Enums;
using Points.Protocol.Models;
using TariffZones.Protocol.Models;

namespace DeliveryDatePlanning.Application.Core.Estimator.CalculationFormulaDefiner;

public abstract class DatesCalculationDefinerChain
{
    protected DatesCalculationDefinerChain Successor;
    
    public DatesCalculationDefinerChain SetSuccessor(DatesCalculationDefinerChain successor)
    {
        this.Successor = successor;

        return this;
    }
    
    public abstract Result<DeliveryDates> DefineFormulaAndCalculateDates(Estimate current, TariffZoneType tariffZoneType, Invoice invoice, 
        Zone zone, Point point, EstimateCommand command);
}