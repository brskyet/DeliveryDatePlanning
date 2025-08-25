using CSharpFunctionalExtensions;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;

public class ReceptionDate : SimpleValueObject<DateTime>
{
    private ReceptionDate(DateTime value) : base(value)
    {
    }

    public static Result<ReceptionDate> Create(DateTime? value)
    {
        if (value.HasValue == false || value == default(DateTime))
        {
            return Result.Failure<ReceptionDate>($"{nameof(ReceptionDate)} should contain a value");
        }
        
        return create((DateTime)value);
    }
    
    public static Result<ReceptionDate> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) || !DateTime.TryParse(value, out var timestamp) || timestamp == default(DateTime))
        {
            return Result.Failure<ReceptionDate>($"{nameof(ReceptionDate)} should contain a value");
        }
        
        
        return create(timestamp);
    }
    
    private static Result<ReceptionDate> create(DateTime value)
    {
        return new ReceptionDate(value);
    }
}