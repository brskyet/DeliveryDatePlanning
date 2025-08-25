using CSharpFunctionalExtensions;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;

public class CityTimeZone : SimpleValueObject<int>
{
    private CityTimeZone(int value) : base(value)
    {
    }
    
    public static Result<CityTimeZone> Create(int value)
    {
        if (value is < -15 or > 9) // т.к. относительно МСК
        {
            return Result.Failure<CityTimeZone>($"{nameof(CityTimeZone)} {nameof(value)} argument is out of range");
        }

        return new CityTimeZone(value);
    }
}
