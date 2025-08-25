using DeliveryDatePlanning.Data.Context;

namespace DeliveryDatePlanning.UnitTest.Common;

public class EstimateContextFactory
{
    public const string InvoiceKeyNotFound = "";
    public const string EncloseKeyNotFound = "";
    public const string InvoiceKeyIsFailure = "";
    public const string EncloseKeyIsFailure = "";
    public const string InvoiceKey = "";
    public const string EncloseKey = "";
    public const string KeyForUpdate = "3141-592";
    
    public static EstimateDbContext Create()
    {
        return default;
    }

    public static void Destroy(EstimateDbContext context)
    {
        
    }
}