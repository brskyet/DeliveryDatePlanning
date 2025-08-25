using CSharpFunctionalExtensions;
using Models;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Enclose;

public class EncloseStateEnum : SimpleValueObject<EncloseState>
{
    private EncloseStateEnum(EncloseState value) : base(value)
    {
    }

    public static Result<EncloseStateEnum> Create(int? value)
    {
        if (value.HasValue == false)
        {
            return Result.Failure<EncloseStateEnum>($"{nameof(EncloseState)} must be instantiated");
        }

        return create((EncloseState) value);
    }

    public static Result<EncloseStateEnum> Create(EncloseState value) 
        => create(value);

    private static Result<EncloseStateEnum> create(EncloseState value)
    {
        if (Enum.IsDefined(typeof(EncloseState), value) == false)
        {
            return Result.Failure<EncloseStateEnum>($"{nameof(GettingType)} does not contains value={value}");
        }

        return new EncloseStateEnum(value);
    }
}