using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Domain.Enums;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.TariffZone;

public class TariffZoneTypeEnum : SimpleValueObject<TariffZoneType>
{
    private TariffZoneTypeEnum(TariffZoneType value) : base(value)
    {
    }
    
    public static Result<TariffZoneTypeEnum> Create(TariffZoneType value)
    {
        if (Enum.IsDefined(typeof(TariffZoneType), value) == false)
        {
            return Result.Failure<TariffZoneTypeEnum>($"{nameof(TariffZoneType)} does not contains value={value}");
        }
        
        return new TariffZoneTypeEnum(value);
    }
}