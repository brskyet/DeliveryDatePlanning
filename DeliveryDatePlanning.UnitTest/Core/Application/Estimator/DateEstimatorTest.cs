using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using DeliveryDatePlanning.Application.Common.Constant;
using DeliveryDatePlanning.Application.Core.Estimator;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.UnitTest.Common;
using DeliveryDatePlanning.UnitTest.Common.TestDataProviders;


namespace DeliveryDatePlanning.UnitTest.Core.Application.Estimator;

public class DateEstimatorTest
{
    [Theory]
    [InlineData(null, 1,3, ExchangeName.EncloseRegistered)]
    public async Task DateEstimation_NoExceptionsDeliveryPointValidTariffZone_Success(List<DateTime> serviceExceptions, int dpMin, int dpMax, string exchangeName)
    {
        // Arrange
        var invoice = Invoices.InvoiceFactoryRedirectionTests("5555-555");
        var point = Common.TestDataProviders.Points.PointFactory(serviceExceptions);
        var zone = Common.TestDataProviders.TariffZones.TariffZonesFactory(dpMin, dpMax);
        var tariffZoneKeys = TariffZoneKeys.GetTariffZoneKeys();
        var estimate = Estimate.CreateDefault(invoice).Value;
        var command = Commands.CommandsFactory(exchangeName);
        var testHelper = new TestHelper();
        var tariffZoneClient = testHelper.GetTariffZonesClientWithProvidedResponse(zone);
        var pointsClient = testHelper.GetPointsClientWithProvidedResponse(point);
        var serviceProvider = new ServiceCollection()
            .AddLogging()
            .BuildServiceProvider();
        var factory = serviceProvider.GetService<ILoggerFactory>();
        var logger = factory.CreateLogger<DateEstimator>();
        var datesEstimator = new DateEstimator(tariffZoneKeys, tariffZoneClient, pointsClient, logger);
        
        // Act
        var result = await datesEstimator.Estimate(invoice, estimate, command);

        // Assert
        Assert.NotNull(result.Value.DateEnd);
        Assert.NotNull(result.Value.DateStart);
    }
}