using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Application.Common.Constant;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;
using DeliveryDatePlanning.Domain.Enums;
using Points.Protocol.Models;
using TariffZones.Protocol.Models;
using EncloseState = Models.EncloseState;

namespace DeliveryDatePlanning.Application.Core.Estimator.CalculationFormulaDefiner;

public class GeneralDeliveryPointZoneCalculationDefiner : DatesCalculationDefinerChain
{
    private readonly IReadOnlyCollection<TariffZoneType> tariffZoneTypes = new[]
    {
        TariffZoneType.GeneralDeliveryPointZone, 
        TariffZoneType.GeneralFivePostDeliveryPointZone,
        TariffZoneType.BC2CGeneralDeliveryPointZone,
        TariffZoneType.BC2CGeneralFivePostDeliveryPointZone,
        TariffZoneType.C2CZone
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
           
            return Result.Failure<DeliveryDates>($"[{nameof(GeneralDeliveryPointZoneCalculationDefiner)}] There is no calculation formula definers to process this estimate. Invoice={invoice.Id}");
        }
        
        var startingDate = command.Exchange is ExchangeName.DayChangedForNotAcceptedInvoiceEstimation or ExchangeName.EncloseAcceptedInDeliveryPoint
            ? command.Timestamp
            : invoice.Encloses.SelectMany(e => e.StateHistory).FirstOrDefault(es => es.State == EncloseState.AcceptedInPT)?.Timestamp;

        if (startingDate is null || startingDate == default(DateTime))
        {
            return current.DeliveryDates;
        }

        var dpMin = zone.DpMin!.Value;
        var dpMax = zone.DpMax!.Value;
        var diff = dpMax - dpMin;
        
        var dateStart = DateOnly.FromDateTime(startingDate.Value)
            .AddDays(dpMin)
            .AddDaysUntilFirstServiceDay(point);
        var dateEnd = dateStart
            .AddDays(diff)
            .AddDaysUntilFirstServiceDay(point);
        
        return DeliveryDates.Create(dateStart, dateEnd);
    }
}