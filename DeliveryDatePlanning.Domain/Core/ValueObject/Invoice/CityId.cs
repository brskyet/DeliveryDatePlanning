using CSharpFunctionalExtensions;
using Models;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;

public class CityId : SimpleValueObject<string>
{
    private CityId(string value) : base(value)
    {
    }
    
    public static Result<CityId> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<CityId>($"{nameof(CityId)} {nameof(value)} argument is null or whitespace");
        }
        
        if (!CompanyKey.TryParse(value, out _))
        {
            return Result.Failure<CityId>($"{nameof(CityId)} is not a valid CompanyKey");
        }
        
        return new CityId(value);
    }
}