using TariffZones.Protocol.Models;

namespace DeliveryDatePlanning.UnitTest.Common.TestDataProviders;

public static class TariffZones
{
    public static Zone TariffZonesFactory(int dpMin, int dpMax)
    {
        return new ()
        {
            DpMin = dpMin,
            DpMax = dpMax
        };
    }
}