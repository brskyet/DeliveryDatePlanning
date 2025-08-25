namespace DeliveryDatePlanning.Application.Declaration.Handler.Query;

public class EstimateReadQuery
{
    public string InvoiceKey { get; }

    public EstimateReadQuery(string invoiceKey)
    {
        this.InvoiceKey = invoiceKey;
    }
}