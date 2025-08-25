using TariffZones.Protocol.Models;

namespace DeliveryDatePlanning.UnitTest.Common.TestDataProviders;

public static class TariffZonesResponses
{
    public static IEnumerable<Zone[]> GetValidTariffZonesResponse
    {
        get
        {
            var result = new List<Zone[]>();
            var innerArray = new Zone[1];

            var zone = new Zone()
            {
                DpMin = 2,
                DpMax = 4
            };

            innerArray[0] = zone;
            result.Add(innerArray);

            return result;
        }
    }
}