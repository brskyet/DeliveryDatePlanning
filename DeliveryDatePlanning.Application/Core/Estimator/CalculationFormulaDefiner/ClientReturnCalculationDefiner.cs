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

public class ClientReturnCalculationDefiner : DatesCalculationDefinerChain
{
    private readonly IReadOnlyCollection<TariffZoneType> tariffZoneTypes = new[]
    {
        TariffZoneType.GeneralReturnsClientReturnZone, TariffZoneType.GeneralFivePostReturnsClientReturnZone
    };

    private readonly IReadOnlyCollection<PostageType> postageTypes = new[] {PostageType.ClientReturn};

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
                return this.Successor.DefineFormulaAndCalculateDates(current, tariffZoneType, invoice, zone, point, command);
            }
            
            return Result.Failure<DeliveryDates>($"[{nameof(ClientReturnCalculationDefiner)}] There is no calculation formula definers to process this estimate. Invoice={invoice.Id}");
        }

        var receivedState =
            invoice.LinkedInvoice.Encloses
                .SelectMany(e => e.StateHistory)
                .FirstOrDefault(es => es.State == EncloseState.Received);

        if (receivedState is null || receivedState.Timestamp == default(DateTime))
        {
            return Result.Failure<DeliveryDates>($"[{nameof(ClientReturnCalculationDefiner)}] ReceivedState is null or has invalid DateTime. Invoice={invoice.Id}");
        }

        var startingDate = command.Exchange == ExchangeName.DayChangedForNotAcceptedInvoiceEstimation || command.Exchange == ExchangeName.EncloseAcceptedInDeliveryPoint
            ? command.Timestamp
            : invoice.LinkedInvoice.Encloses
                .SelectMany(e => e.StateHistory)
                .FirstOrDefault(es =>
                    es.State == EncloseState.AcceptedInPT &&
                    es.Timestamp > receivedState.Timestamp)?.Timestamp;

        if (startingDate is null)
        {
            return current.DeliveryDates;
        }

        var dpMin = zone.DpMin!.Value;
        var dpMax = zone.DpMax!.Value;
        var diff  = dpMax - dpMin;

        var dateStart = DateOnly.FromDateTime(startingDate.Value)
            .AddDays(dpMin)
            .AddDaysUntilFirstServiceDay(point);
        var dateEnd = dateStart
            .AddDays(diff)
            .AddDaysUntilFirstServiceDay(point);

        return DeliveryDates.Create(dateStart, dateEnd);
    }
}