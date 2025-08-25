using CSharpFunctionalExtensions;
using Models;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;

public class PostageTypeEnum : SimpleValueObject<PostageType>
{
    private PostageTypeEnum(PostageType value) : base(value)
    {
    }
    
    public static Result<PostageTypeEnum> Create(PostageType? value)
    {
        if (value.HasValue == false)
        {
            return Result.Failure<PostageTypeEnum>($"{nameof(PostageType)} must be instantiated");
        }

        if (Enum.IsDefined(typeof(PostageType), value) == false)
        {
            return Result.Failure<PostageTypeEnum>($"{nameof(PostageType)} does not contains value={value}");
        }
        
        return new PostageTypeEnum(value.Value);
    }
}