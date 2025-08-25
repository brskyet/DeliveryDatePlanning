using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Application.Common.Constant;
using DeliveryDatePlanning.Application.Declaration.Estimator;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;
using DeliveryDatePlanning.Domain.Enums;

namespace DeliveryDatePlanning.Application.Core.Estimator;

public class DateChangeReasonEstimator : IDateChangeReasonEstimator
{
    public Result<DateChangeReasonTypeEnum> Estimate(EstimateCommand command, Invoice invoice, Estimate current, Estimate previousEstimate)
    {
        if (this.IsDatesHasBeenChanged(current, previousEstimate) == false)
            return current.Reason;

        var recipientCityChanged = invoice.CityTo.Value != current.Invoice.CityTo.Value;
        var senderCityChanged = invoice.CityFrom.Value != current.Invoice.CityFrom.Value;
        var recipientDeliveryPointChanged = invoice.RecipientPointNumber.Value != current.Invoice.RecipientPointNumber.Value;
        var deliveryDateIsEmpty = current.IsDeliveryDateEmpty() || 
                                  ((command.Exchange == ExchangeName.InvoiceRedirected || command.Exchange == ExchangeName.InvoiceSenderCityChanged)
                                   && !current.IsDeliveryDateEmpty() 
                                   && previousEstimate.DeliveryDates.DateStart is null 
                                   && previousEstimate.DeliveryDates.DateEnd is null);

        const string forbiddenApt = "0001-001";
        var dateChangeReason = (command.Exchange, deliveryDateIsEmpty, 
                                current.Invoice.RecipientPointNumber.Value, 
                                invoice.AllEnclosesInInitialStates(), recipientCityChanged, recipientDeliveryPointChanged, senderCityChanged) 
        switch
        {
            
            (ExchangeName.InvoicePickupDateChanged, false, _, _, false, false, false) => DateChangeReasonType.ReceptionDateChanged,
            (ExchangeName.InvoicePickupDateChanged, false, _, _, true, _, _) => DateChangeReasonType.RecipientCityChanged,
            (ExchangeName.InvoicePickupDateChanged, false, _, _, false, true, _) => DateChangeReasonType.Forwarded,
            (ExchangeName.InvoiceSenderCityChanged, false, _,  _, _, _, true) => DateChangeReasonType.SenderCityChanged,
            (ExchangeName.InvoiceRecipientCityChanged, false, _,  _, true, _, false) => DateChangeReasonType.RecipientCityChanged,
            (ExchangeName.InvoiceRedirected, true, forbiddenApt,  _, true, _, false) => DateChangeReasonType.ReceptionDateChanged,
            (ExchangeName.InvoiceRedirected, false, _, _, true, _, false) => DateChangeReasonType.RecipientCityChanged, 
            (ExchangeName.InvoiceRedirected, false, not forbiddenApt, _, false, true, false) => DateChangeReasonType.Forwarded,
            (ExchangeName.InvoiceRedirected, true, forbiddenApt,  _, false, true, false) => DateChangeReasonType.Forwarded,
            (ExchangeName.InvoiceRecipientCityChanged, true, forbiddenApt,  _, false, true, false) => DateChangeReasonType.Forwarded,
            (ExchangeName.InvoiceRecipientCityChanged, false, not forbiddenApt, _, false, true, false) => DateChangeReasonType.Forwarded,
            (ExchangeName.DayChangedForNotAcceptedInvoiceEstimation, _, _, true, false, _, _) => DateChangeReasonType.ItemsNotProvidedByClient,
            _ => current.Reason
        };
        
       return DateChangeReasonTypeEnum.Create(dateChangeReason);
    }
    
    private bool IsDatesHasBeenChanged(Estimate current, Estimate previousEstimation)
    {
        var newDateStartValue = current.DeliveryDates.DateStart; 
        var newDateEndValue = current.DeliveryDates.DateEnd; 
        var oldDateStartValue = previousEstimation?.DeliveryDates.DateStart;
        var oldDateEndValue = previousEstimation?.DeliveryDates.DateEnd;
        
        return
            (newDateStartValue != null && oldDateStartValue != newDateStartValue)
            || (newDateEndValue != null && oldDateEndValue != newDateEndValue);
    }
}