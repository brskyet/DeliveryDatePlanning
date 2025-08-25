using CSharpFunctionalExtensions;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;

public class OverdueDays : SimpleValueObject<int?>
{
    private OverdueDays(int? value) : base(value)
    {
    }
    
    public static Result<OverdueDays> Create(int? value)
    {
        if (value is <= 0)
        {
            return Result.Failure<OverdueDays>($"{nameof(OverdueDays)} is out of range. Value={value}");
        }

        return new OverdueDays(value);
    }
    
    public static Result<OverdueDays> CreateDefault()
    {
        return new OverdueDays(null);
    }
}