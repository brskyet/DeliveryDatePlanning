using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Domain.Enums;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;

public class EstimateStatusTypeEnum : SimpleValueObject<EstimateStatusType>
{
    private EstimateStatusTypeEnum(EstimateStatusType value) : base(value)
    {
    }
    
    public static Result<EstimateStatusTypeEnum> Create(EstimateStatusType value)
    {
        if (Enum.IsDefined(typeof(EstimateStatusType), value) == false)
        {
            return Result.Failure<EstimateStatusTypeEnum>($"{nameof(EstimateStatusType)} does not contains value={value}");
        }
        
        return new EstimateStatusTypeEnum(value);
    }
    
    public static Result<EstimateStatusTypeEnum> CreateDefault()
    {
        var defaultValue = EstimateStatusType.None;
        return new EstimateStatusTypeEnum(defaultValue);
    }
}