using System.Text.Json;

namespace DeliveryDatePlanning.Application.Declaration.Store;

public class EstimateFilter
{
    public string InvoiceKey { get; }
    
    public string EncloseKey { get; }

    public EstimateFilter(string invoiceKey, string encloseKey)
    {
        this.InvoiceKey = invoiceKey;
        this.EncloseKey = encloseKey;
    }

    public static EstimateFilter ByInvoiceKey(string invoiceKey)
    {
        return new EstimateFilter(invoiceKey, null);
    }
    
    public static EstimateFilter ByEncloseKey(string encloseKey)
    {
        return new EstimateFilter(null, encloseKey);
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}