using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;

public class ContractNumber : SimpleValueObject<string>
{
    private ContractNumber(string value) : base(value)
    {
    }
    
    public static Result<ContractNumber> Create(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Result.Failure<ContractNumber>($"{nameof(value)} argument is null or whitespace");
        }
        
        if (Regex.IsMatch(value, @"[9]\d{9}") == false)
        {
            return Result.Failure<ContractNumber>($"{nameof(value)} format is invalid");
        }
        
        return new ContractNumber(value);
    }
}