namespace DeliveryDatePlanning.Application.Declaration.Publisher;

public class DayChangedEstimationEvent
{
    public string InvoiceKey { get; }
    
    public DateTime Timestamp { get; }

    public DayChangedEstimationEvent(string invoiceKey, DateTime timestamp)
    {
        this.InvoiceKey = invoiceKey ?? throw new ArgumentNullException(nameof(invoiceKey));
        this.Timestamp = timestamp;
    }
}