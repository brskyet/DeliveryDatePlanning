using DeliveryDatePlanning.Application.Declaration.Store;
using DeliveryDatePlanning.Data.Context;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.TariffZone;
using DeliveryDatePlanning.Domain.Enums;
using Models;

namespace DeliveryDatePlanning.Data.Store;

public class TariffZoneKeyStore : ITariffZoneKeyStore
{   
    private readonly AptContext context;

    public TariffZoneKeyStore(AptContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public TariffZoneKey[] All()
    {
        var dbEntities = context.TariffZones.ToList();
        
        var zoneTypes = ((TariffZoneType[]) Enum.GetValues(typeof(TariffZoneType)))
            .Where(type => type != TariffZoneType.None)
            .ToArray();
        
        var collection = new List<TariffZoneKey>();
            
        foreach (var zoneType in zoneTypes)
        {
            var zone = dbEntities.Single(z => z.Comment == zoneType.GetDescription());

            collection.Add(TariffZoneKey.Create(new CompanyKey(zone.Id, zone.OwnerId).ToString(), TariffZoneTypeEnum.Create(zoneType).Value).Value);
        }

        return collection.ToArray();
    }
}