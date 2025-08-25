using System.Text.Encodings.Web;
using System.Text.Json;
using CSharpFunctionalExtensions;
using Microsoft.Extensions.Logging;
using DeliveryDatePlanning.Application.Core.Estimator.CalculationFormulaDefiner;
using DeliveryDatePlanning.Application.Declaration.Estimator;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;
using DeliveryDatePlanning.Domain.Enums;
using Models;
using Points.Protocol;
using Points.Protocol.Models;
using TariffZones.Protocol;
using TariffZones.Protocol.Models;

namespace DeliveryDatePlanning.Application.Core.Estimator;

public class DateEstimator : IDateEstimator
{
    private readonly TariffZonesClient tariffZonesClient;
    private readonly PointsClient pointsClient;
    private readonly IReadOnlyCollection<TariffZoneKey> tariffZoneKeys;
    private readonly DatesCalculationDefinerChain chain;
    private readonly ILogger<DateEstimator> logger;
    
    public DateEstimator(IReadOnlyCollection<TariffZoneKey> tariffZoneKeys, TariffZonesClient tariffZonesClient, PointsClient pointsClient, ILogger<DateEstimator> logger)
    {
        this.tariffZonesClient = tariffZonesClient;
        this.pointsClient = pointsClient;
        this.tariffZoneKeys = tariffZoneKeys;
        this.logger = logger;

        //Configure chain
        this.chain = new GeneralZoneCalculationDefiner()
            .SetSuccessor(new GeneralDeliveryPointZoneCalculationDefiner()
                .SetSuccessor(new GeneralReturnsCalculationDefiner()
                    .SetSuccessor(new PostReturnCalculationDefiner()
                        .SetSuccessor(new ClientReturnCalculationDefiner()
                        ))));
    }
    
    public async Task<Result<DeliveryDates>> Estimate(Invoice invoice, Estimate current, EstimateCommand command, CancellationToken token = default)
    {
        if (current == null)
        {
            return Result.Failure<DeliveryDates>($"[{nameof(DateEstimator)}] {nameof(current)} estimate entity must be created");
        }
        
        var tariffZoneType = invoice.DefineTariffZoneType();
        if (tariffZoneType == TariffZoneType.None)
            return current.DeliveryDates;
        
        //get data for calculation
        var tariffZoneKey = this.tariffZoneKeys.First(tz => tz.ZoneType == tariffZoneType);

        var tariffZonesRequest = new ZoneQuery()
        {
            Priority = tariffZoneType == TariffZoneType.C2CZone ? null : invoice.DeliveryMode.Value,
            CityFromId = invoice.CityFrom.Value,
            CityToId = invoice.CityTo.Value,
            TariffZoneId = tariffZoneKey.Id
        };

        var tariffZonesResponse = await this.tariffZonesClient.GetZone(tariffZonesRequest);
        
        this.logger.LogInformation("{Record}",
            JsonSerializer.Serialize(
                new
                {
                    InvoiceKey = invoice.Id,
                    TariffZoneType = tariffZoneType.GetDescription(),
                    TariffZonesRequest = tariffZonesRequest,
                    TariffZonesResponse = tariffZonesResponse
                },
                new JsonSerializerOptions()
                {
                    Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                }));

                                                                                            
        if (tariffZonesResponse is null)
        {
            return Result.Failure<DeliveryDates>($"[{nameof(DateEstimator)}] TariffZonesResponse is null. Request={JsonSerializer.Serialize(tariffZonesRequest)}");
        }
        
        var hasNoTariffZone = tariffZonesResponse.DpMin is null or < 1 || tariffZonesResponse.DpMax is null or < 1;
        
        if (hasNoTariffZone)
        {
            return current.DeliveryDates;
        }
      
        var deliveryPointNumber = invoice.RecipientPointNumber;

        Point point = null;
        
        if (!string.IsNullOrWhiteSpace(deliveryPointNumber.Value))
        {
            point = await this.pointsClient.GetPointsAsync(deliveryPointNumber);
        
            if (point is null)
            {
                return Result.Failure<DeliveryDates>( $"[{nameof(DateEstimator)}] Points response is null. Point={deliveryPointNumber}");
            }
        }
        
        //calculate dates via chain of responsibility
        return this.chain.DefineFormulaAndCalculateDates(current, tariffZoneKey.ZoneType, invoice, tariffZonesResponse, point, command );
    }
}
