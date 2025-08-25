namespace DeliveryDatePlanning.Application.Declaration.Handler.Command;

public class DayChangedPublishingCommand
{
    public DateTime Timestamp { get; }

    public DayChangedPublishingCommand(DateTime timestamp)
    {
        Timestamp = timestamp;
    }
}