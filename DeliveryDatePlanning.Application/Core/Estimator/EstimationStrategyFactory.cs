using Microsoft.Extensions.Logging;
using DeliveryDatePlanning.Application.Declaration.Estimator;
using DeliveryDatePlanning.Domain.Core.Entity;
using Points.Protocol;
using TariffZones.Protocol;

namespace DeliveryDatePlanning.Application.Core.Estimator;

public class EstimationStrategyFactory : IEstimationStrategyFactory
{
    private readonly IReadOnlyCollection<TariffZoneKey> tariffZoneKeys;
    private readonly TariffZonesClient tariffZonesClient;
    private readonly PointsClient pointsClient;
    private readonly ILogger<DateEstimator> dateEstimatorLogger;

    public EstimationStrategyFactory(IReadOnlyCollection<TariffZoneKey> tariffZoneKeys, TariffZonesClient tariffZonesClient, PointsClient pointsClient, ILogger<DateEstimator> dateEstimatorLogger)
    {
        this.tariffZoneKeys = tariffZoneKeys;
        this.tariffZonesClient = tariffZonesClient;
        this.pointsClient = pointsClient;
        this.dateEstimatorLogger = dateEstimatorLogger;
    }
    
    public IEstimationStrategy OnlyStatus()
    {
        var statusEstimator = new StatusEstimator();
        var overdueEstimator = new OverdueEstimator();
        
        return new OnlyStatusEstimationStrategy(statusEstimator, overdueEstimator);
    }

    public IEstimationStrategy DateAndStatus()
    {
        var dateEstimator = new DateEstimator(this.tariffZoneKeys, this.tariffZonesClient, this.pointsClient, this.dateEstimatorLogger);
        var statusEstimator = new StatusEstimator();
        var overdueEstimator = new OverdueEstimator();
        var dateChangeReasonEstimator = new DateChangeReasonEstimator();
        
        return new DateAndStatusEstimationStrategy(dateEstimator, statusEstimator, overdueEstimator, dateChangeReasonEstimator);
    }
}