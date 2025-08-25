using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Domain.Enums;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;

public class DateChangeReasonTypeEnum : SimpleValueObject<DateChangeReasonType>
{
    private DateChangeReasonTypeEnum(DateChangeReasonType value) : base(value)
    {
    }
    
    public static Result<DateChangeReasonTypeEnum> Create(DateChangeReasonType value)
    {
        if (Enum.IsDefined(typeof(DateChangeReasonType), value) == false)
        {
            return Result.Failure<DateChangeReasonTypeEnum>($"{nameof(DateChangeReasonType)} does not contains value={value}");
        }
        
        return new DateChangeReasonTypeEnum(value);
    }
    
    public static Result<DateChangeReasonTypeEnum> CreateDefault()
    {
        var defaultValue = DateChangeReasonType.None;
        
        return new DateChangeReasonTypeEnum(defaultValue);
    }
}