using CSharpFunctionalExtensions;
using Models;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;

public class GettingTypeEnum : SimpleValueObject<GettingType>
{
    private GettingTypeEnum(GettingType value) : base(value)
    {
    }
    
    public static Result<GettingTypeEnum> Create(GettingType? value)
    {
        if (value.HasValue == false)
        {
            return Result.Failure<GettingTypeEnum>($"{nameof(GettingType)} must be instantiated");
        }

        if (Enum.IsDefined(typeof(GettingType), value) == false)
        {
            return Result.Failure<GettingTypeEnum>($"{nameof(GettingType)} does not contains value={value}");
        }
        
        return new GettingTypeEnum(value.Value);
    }
}