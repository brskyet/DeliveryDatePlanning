namespace DeliveryDatePlanning.Application.Declaration.Handler.Command;

public class InvoiceDataChangedCommand
{
    public string Key { get; }
    public string Exchange { get; }

    public InvoiceDataChangedCommand(string key, string exchange)
    {
        Key = key;
        Exchange = exchange;
    }
}