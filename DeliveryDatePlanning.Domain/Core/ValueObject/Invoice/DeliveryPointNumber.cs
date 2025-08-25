using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;

public class DeliveryPointNumber : SimpleValueObject<string>
{
    private DeliveryPointNumber(string value) : base(value)
    {
    }
    
    public static Result<DeliveryPointNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value) == false && Regex.IsMatch(value, @"(^[0-9]+)[-]([0-9]+$)") == false)
        {
            return Result.Failure<DeliveryPointNumber>($"{nameof(value)} format is invalid");
        }
        
        return new DeliveryPointNumber(value);
    }
}