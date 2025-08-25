using System.Net;
using System.Text.Json;
using Moq;
using Moq.Protected;
using Points.Protocol;
using Points.Protocol.Models;
using TariffZones.Protocol;
using TariffZones.Protocol.Models;

namespace DeliveryDatePlanning.UnitTest.Common;

public class TestHelper
{

    public TariffZonesClient GetTariffZonesClientWithProvidedResponse(Zone response)
    {
        var tariffZonesHttpMessageHandler = this.GetMockHttpMessageHandler(response);
        var httpClientForZones = new HttpClient(tariffZonesHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://test.com/"),
        };
        return new TariffZonesClient(httpClientForZones);
    }
    
    public PointsClient GetPointsClientWithProvidedResponse(Point response)
    {
        var pointsHttpMessageHandler = this.GetMockHttpMessageHandler(response);
        var httpClientPoints = new HttpClient(pointsHttpMessageHandler.Object)
        {
            BaseAddress = new Uri("http://tests.com/"),
        };
        return new PointsClient(httpClientPoints);
    }
    
    private Mock<HttpMessageHandler> GetMockHttpMessageHandler<T>(T response)
    {
        var handlerMock = new Mock<HttpMessageHandler>(MockBehavior.Strict);
        handlerMock
            .Protected()
            .Setup<Task<HttpResponseMessage>>(
                "SendAsync",
                ItExpr.IsAny<HttpRequestMessage>(),
                ItExpr.IsAny<CancellationToken>()
            )
            // prepare the expected response of the mocked http call
            .ReturnsAsync(new HttpResponseMessage()
            {
                StatusCode = HttpStatusCode.OK,
                Content = new StringContent(JsonSerializer.Serialize(response)),
            })
            .Verifiable();
        return handlerMock;
    }
}