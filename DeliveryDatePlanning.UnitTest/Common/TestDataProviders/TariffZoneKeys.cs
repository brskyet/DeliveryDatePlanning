using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.TariffZone;
using DeliveryDatePlanning.Domain.Enums;
using Models;

namespace DeliveryDatePlanning.UnitTest.Common.TestDataProviders;

public static class TariffZoneKeys
{
    public static List<TariffZoneKey> GetTariffZoneKeys()
    {
        var zoneTypes = ((TariffZoneType[]) Enum.GetValues(typeof(TariffZoneType)))
            .Where(type => type != TariffZoneType.None)
            .ToArray();
        
        var collection = new List<TariffZoneKey>();

        var i = 1;
        foreach (var zoneType in zoneTypes)
        {
            collection.Add(TariffZoneKey.Create(new CompanyKey(i++, i).ToString(), TariffZoneTypeEnum.Create(zoneType).Value).Value);
        }

        return collection;
    }
}