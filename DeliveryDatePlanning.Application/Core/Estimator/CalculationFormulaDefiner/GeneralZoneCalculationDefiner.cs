using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Application.Common.Constant;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;
using DeliveryDatePlanning.Domain.Enums;
using Points.Protocol.Models;
using TariffZones.Protocol.Models;

namespace DeliveryDatePlanning.Application.Core.Estimator.CalculationFormulaDefiner;

public class GeneralZoneCalculationDefiner : DatesCalculationDefinerChain
{
    private readonly IReadOnlyCollection<TariffZoneType> tariffZoneTypes = new[]
    {
        TariffZoneType.GeneralCourierWarehouseZone, TariffZoneType.GeneralFivePostCourierWarehouseZone
    };

    public override Result<DeliveryDates> DefineFormulaAndCalculateDates(
        Estimate current,
        TariffZoneType tariffZoneType,
        Invoice invoice,
        Zone zone,
        Point point,
        EstimateCommand command)
    {
        if (!tariffZoneTypes.Contains(tariffZoneType))
        {
            if (this.Successor is not null)
            {
                return this.Successor.DefineFormulaAndCalculateDates(current, tariffZoneType, invoice, zone, point, command);
            }
            
            return Result.Failure<DeliveryDates>($"[{nameof(GeneralZoneCalculationDefiner)}] There is no calculation formula definers to process this estimate. Invoice={invoice.Id}");
        }
        
        
        var startingDate = command.Exchange == ExchangeName.DayChangedForNotAcceptedInvoiceEstimation
                         ? command.Timestamp
                         : invoice.ReceptionDate.Value;
        
        var dpMin = zone.DpMin!.Value;
        var dpMax = zone.DpMax!.Value;
        var diff = dpMax - dpMin;
        
        var dateStart= DateOnly.FromDateTime(startingDate)
            .AddDays(dpMin)
            .AddDaysUntilFirstServiceDay(point);
        var dateEnd = dateStart
            .AddDays(diff)
            .AddDaysUntilFirstServiceDay(point);
        
        return DeliveryDates.Create(dateStart, dateEnd);
    }
}