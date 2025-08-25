using CSharpFunctionalExtensions;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;

public class DeliveryPointJurName : SimpleValueObject<string>
{
    private DeliveryPointJurName(string value) : base(value)
    {
    }
    
    public static Result<DeliveryPointJurName> Create(string value)
    {
        return new DeliveryPointJurName(value);
    }
}