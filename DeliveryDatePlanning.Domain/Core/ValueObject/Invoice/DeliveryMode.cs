using CSharpFunctionalExtensions;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;

public class DeliveryMode : SimpleValueObject<int>
{
    private DeliveryMode(int value) : base(value)
    {
    }
    
    public static Result<DeliveryMode> Create(int? value)
    {
        if (value.HasValue & value is < 0 or > 2)
        {
            return Result.Failure<DeliveryMode>($"{nameof(DeliveryMode)} is out of range. Value={value}");
        }
        
        if (value.HasValue == false || value is 0)
        {
            return new DeliveryMode(1);
        }

        return new DeliveryMode(value.Value);
    }
}