using CSharpFunctionalExtensions;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;

public class DeliveryDates : CSharpFunctionalExtensions.ValueObject
{
    public DateOnly? DateStart { get; }
    public DateOnly? DateEnd { get; }
    
    private DeliveryDates(DateOnly? dateStart, DateOnly? dateEnd)
    {
        this.DateStart = dateStart;
        this.DateEnd = dateEnd;
    }

    public static Result<DeliveryDates> Create(DateOnly? dateStart, DateOnly? dateEnd)
    {
        if (dateStart is null && dateEnd is not null
            || dateStart is not null && dateEnd is null)
        {
            return Result.Failure<DeliveryDates>($"Both {nameof(DateStart)} and {nameof(DateEnd)} should have or dont have values.");
        }
        
        return new DeliveryDates(dateStart, dateEnd);
    }

    public static Result<DeliveryDates> CreateDefault()
    { 
        return new DeliveryDates(null, null);
    }
    
    public bool IsEmpty()
    {
        return this.DateStart == default && this.DateEnd == default;
    }
    
    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return this.DateStart;
        yield return this.DateEnd;
    }
}