using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Domain.Core.ValueObject.TariffZone;

namespace DeliveryDatePlanning.Domain.Core.Entity;

public class TariffZoneKey : Entity<string>
{
    public TariffZoneTypeEnum ZoneType { get; }
    
    private TariffZoneKey(string id, TariffZoneTypeEnum zoneType) : base(id)
    {
        this.ZoneType = zoneType;
    }

    public static Result<TariffZoneKey> Create(string id, TariffZoneTypeEnum zoneType)
    {
        var entity = new TariffZoneKey(id, zoneType);

        return entity;
    }
}