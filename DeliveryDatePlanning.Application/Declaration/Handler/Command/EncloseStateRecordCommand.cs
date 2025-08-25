using Models;

namespace DeliveryDatePlanning.Application.Declaration.Handler.Command;

public class EncloseStateRecordCommand
{
    public string Key { get; }
    
    public DateTime Timestamp { get; }
    
    public EncloseState State { get; }

    public EncloseStateRecordCommand(string key, DateTime timestamp, EncloseState state)
    {
        this.Key = key ?? throw new ArgumentNullException(nameof(key));
        this.Timestamp = timestamp;
        this.State = state;
    }
}