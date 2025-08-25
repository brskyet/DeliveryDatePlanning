using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Application.Common.Constant;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;
using DeliveryDatePlanning.Domain.Enums;
using Models;
using Points.Protocol.Models;
using TariffZones.Protocol.Models;
using EncloseState = Models.EncloseState;

namespace DeliveryDatePlanning.Application.Core.Estimator.CalculationFormulaDefiner;

public class GeneralReturnsCalculationDefiner : DatesCalculationDefinerChain
{
    private readonly IReadOnlyCollection<TariffZoneType> tariffZoneTypes = new[]
    {
        TariffZoneType.GeneralReturnsZone, TariffZoneType.GeneralFivePostReturnsZone, TariffZoneType.BC2CReturnGeneralDeliveryPointZone,
        TariffZoneType.BC2CReturnGeneralFivePostDeliveryPointZone, TariffZoneType.BC2CReturnGeneralZone
    };

    private readonly IReadOnlyCollection<PostageType> postageTypes = new[]
    {
        PostageType.Return, PostageType.ReturnWithCashOnDelivery, PostageType.ReturnBC2C, PostageType.ReturnBC2CWithCashOnDelivery
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
            if (this.Successor is not null && !postageTypes.Contains(invoice.PostageType))
            {
                return this.Successor.DefineFormulaAndCalculateDates(current,tariffZoneType, invoice, zone, point, command);
            }
            
            return Result.Failure<DeliveryDates>($"[{nameof(GeneralReturnsCalculationDefiner)}] There is no calculation formula definers to process this estimate. Invoice={invoice.Id}");
        } 
        
        var startingDate = command.Exchange == ExchangeName.DayChangedForNotAcceptedInvoiceEstimation 
             ? command.Timestamp
             : invoice?.LinkedInvoice?.Encloses
                .SelectMany(e => e.StateHistory)
                .OrderBy(e => e.Timestamp)
                .FirstOrDefault(es => es.State is EncloseState.Rejected or EncloseState.Unclaimed)?.Timestamp;

        if (startingDate == null ||  startingDate == default(DateTime))
        {
            return Result.Failure<DeliveryDates>($"[{nameof(GeneralReturnsCalculationDefiner)}] Cant define starting date - {startingDate}. Invoice={invoice.Id}");
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