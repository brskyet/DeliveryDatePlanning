using CSharpFunctionalExtensions;
using Models;

namespace DeliveryDatePlanning.Domain.Core.ValueObject.Enclose;

public class EncloseStateHistoryRecord : CSharpFunctionalExtensions.ValueObject
{
    public DateTime Timestamp { get; }
    public EncloseState State { get; }
    
    private EncloseStateHistoryRecord(DateTime timestamp, EncloseState state)
    {
        this.Timestamp = timestamp;
        this.State = state;
    }
    
    public static Result<EncloseStateHistoryRecord> Create(DateTime timestamp, int? state)
    {
        if (state.HasValue == false)
        {
            return Result.Failure<EncloseStateHistoryRecord>($"{nameof(EncloseState)} must be instantiated");
        }

        return create(timestamp, (EncloseState) state);
    }
    
    
    public static Result<EncloseStateHistoryRecord> Create(string timestamp, EncloseState state)
    {
        if (string.IsNullOrWhiteSpace(timestamp) || !DateTime.TryParse(timestamp, out var timestampValue))
        {
            return Result.Failure<EncloseStateHistoryRecord>($"{nameof(Timestamp)} must be instantiated");
        }
        
        return create(timestampValue, state);
    }
    
    public static Result<EncloseStateHistoryRecord> Create(DateTime timestamp, EncloseState state) =>
        create(timestamp, state);
    
    private static Result<EncloseStateHistoryRecord> create(DateTime timestamp, EncloseState state)
    {
        if (Enum.IsDefined(typeof(EncloseState), state) == false)
        {
            return Result.Failure<EncloseStateHistoryRecord>($"{nameof(EncloseState)} does not contains value={state}");
        }
        
        var entity = new EncloseStateHistoryRecord(timestamp, state);

        return entity;
    }

    protected override IEnumerable<object> GetEqualityComponents()
    {
        yield return Timestamp;
        yield return State;
    }
}