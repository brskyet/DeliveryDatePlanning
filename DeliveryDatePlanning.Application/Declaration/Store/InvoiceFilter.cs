using System.Text.Json;

namespace DeliveryDatePlanning.Application.Declaration.Store;

public class InvoiceFilter
{
    public string InvoiceKey { get; }
    
    public string EncloseKey { get; }

    public InvoiceFilter(string invoiceKey, string encloseKey)
    {
        this.InvoiceKey = invoiceKey;
        this.EncloseKey = encloseKey;
    }
    
    public static InvoiceFilter ByInvoiceKey(string invoiceKey)
    {
        return new InvoiceFilter(invoiceKey, null);
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}