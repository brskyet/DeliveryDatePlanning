namespace DeliveryDatePlanning.Application.Common.Constant;

public static class ExchangeName
{
    public const string EncloseRegistered = "101";
    public const string EncloseDeliveredToDeliveryPoint = "109";
    public const string EncloseAcceptedInDeliveryPoint = "110";
    public const string EncloseReceived = "111";
    public const string EncloseReturnedToClient = "113";
    public const string EncloseRejected = "114";
    public const string EncloseReadyForReturn = "115";
    public const string EncloseAnnulled = "128";
    public const string InvoicePickupDateChanged = "1142";
    public const string InvoiceSenderCityChanged = "1125";
    public const string InvoiceRedirected = "1102";
    public const string InvoiceRecipientCityChanged = "1137";
    public const string InvoiceUtilization = "1131";
    public const string InvoiceSendToReturn = "1127";
    public const string InvoiceDeliveryModeChanged = "1143";

    public const string DayChangedForStatusEstimation = "DayChanged_StatusEstimation";
    public const string DayChangedForOverdueEstimation = "DayChanged_OverdueEstimation";
    public const string DayChangedForNotAcceptedInvoiceEstimation = "DayChanged_NotAcceptedInvoiceEstimation";
}