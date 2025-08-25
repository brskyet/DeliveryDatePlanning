using System.Text.Json;
using DeliveryDatePlanning.Application.Common.Constant;

namespace DeliveryDatePlanning.Application.Declaration.Handler.Command;

public class EstimateCommand
{
    public string InvoiceKey { get; private set; }
    
    public string EncloseKey { get; private set; }
    
    public DateTime Timestamp { get; }
    
    public string Exchange { get; }
    
    public bool IsInvoiceReceptionDateChanged => this.Exchange == ExchangeName.InvoicePickupDateChanged;

    private readonly IReadOnlyCollection<string> invoiceDataChangedIsTrueExchanges = new[]
    {
        ExchangeName.InvoicePickupDateChanged, ExchangeName.InvoiceSenderCityChanged,
        ExchangeName.InvoiceRedirected, ExchangeName.InvoiceRecipientCityChanged
    };
    public bool IsInvoiceDataChanged => this.invoiceDataChangedIsTrueExchanges.Contains(Exchange);

    public bool ShouldProcessLinkedInvoice => this.Exchange == ExchangeName.EncloseReadyForReturn;
    
    public EstimateCommand(DateTime timestamp, string exchange)
    {
        this.Exchange = exchange ?? throw new ArgumentNullException(nameof(exchange));
        this.Timestamp = timestamp;
    }

    public EstimateCommand SetInvoice(string value)
    {
        this.InvoiceKey = value ?? throw new ArgumentNullException(nameof(value));
        
        return this;
    }
    
    public EstimateCommand SetEnclose(string value)
    {
        this.EncloseKey = value ?? throw new ArgumentNullException(nameof(value));
        
        return this;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}