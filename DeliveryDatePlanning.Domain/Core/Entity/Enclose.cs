using System.Text.RegularExpressions;
using DeliveryDatePlanning.Domain.Core.ValueObject.Enclose;
using Models;

namespace DeliveryDatePlanning.Domain.Core.Entity;

using CSharpFunctionalExtensions;

public class Enclose : Entity<string>
{
    public EncloseStateEnum State { get; private set; }
    public IReadOnlyCollection<EncloseStateHistoryRecord> StateHistory { get; private set; }

    private Enclose(string id, EncloseStateEnum state, IReadOnlyCollection<EncloseStateHistoryRecord> stateHistory) : base(id)
    {
        this.State = state;
        this.StateHistory = stateHistory;
    }
    
    public static Result<Enclose> Create(string id, EncloseStateEnum state, EncloseStateHistoryRecord[] stateHistory)
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return Result.Failure<Enclose>($"{nameof(id)} argument is null or whitespace");
        }
        
        if (Regex.IsMatch(id, @"(^[0-9]+)[-]([0-9]+$)") == false)
        {
            return Result.Failure<Enclose>($"{nameof(id)} format is invalid");
        }

        var history = stateHistory ?? Array.Empty<EncloseStateHistoryRecord>();

        var entity = new Enclose(id, state, history);

        return entity;
    }
    
    public static Result<Enclose> Create(Enclose src)
    {
        var id = src.Id;
        var state = EncloseStateEnum.Create(src.State).Value;
        var encloseHistory = src.StateHistory ?? Array.Empty<EncloseStateHistoryRecord>();;

        var entity = new Enclose(id, state, encloseHistory);

        return entity;
    }
    
    public Enclose AddEncloseStateRecord(DateTime timestamp, EncloseState state)
    {
        var record = EncloseStateHistoryRecord.Create(timestamp, state).Value;
        
        StateHistory = StateHistory.Concat(new []{ record }).ToArray();

        return this;
    }
    
    public Enclose SetState(EncloseState value)
    {
        var state = EncloseStateEnum.Create(value).Value;
        
        State = state;

        return this;
    }
}