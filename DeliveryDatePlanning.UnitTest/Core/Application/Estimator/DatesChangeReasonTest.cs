using DeliveryDatePlanning.Application.Common.Constant;
using DeliveryDatePlanning.Application.Core.Estimator;
using DeliveryDatePlanning.Domain.Enums;
using DeliveryDatePlanning.UnitTest.Common.TestDataProviders;

namespace DeliveryDatePlanning.UnitTest.Core.Application.Estimator;

public class DatesChangeReasonTest
{
    [Theory]
    [InlineData(ExchangeName.InvoicePickupDateChanged, false, false, true)]
    public void DateChangeReason_InvoicePickupDateChanged_Success (string exchangeName, bool forbiddenPointInvoicePrevious, bool previousDateIsEmpty ,bool datesHasBeenChanged)
    {
        // Arrange
        var reasonEstimator = new DateChangeReasonEstimator();
        var command = Commands.CommandsFactory(exchangeName);
        var invoice = Invoices.InvoiceFactoryRedirectionTests(forbiddenPointInvoicePrevious ? "0001-001" : "1111-111");
        var invoice2 = Invoices.InvoiceFactoryRedirectionTests(forbiddenPointInvoicePrevious ? "0001-001" : "2222-222");
        var current = Estimates.EstimateFactory(invoice, false, previousDateIsEmpty, datesHasBeenChanged);
        var previous = Estimates.EstimateFactory(invoice2, true, previousDateIsEmpty, datesHasBeenChanged);
        
        // Act
        var result =  reasonEstimator.Estimate(command, invoice, current, previous);

        // Assert
        Assert.Equal(DateChangeReasonType.ReceptionDateChanged, result.Value.Value);
    }
    
    [Theory]
    [InlineData(ExchangeName.InvoiceDeliveryModeChanged, false, false, false)]
    [InlineData(ExchangeName.InvoiceDeliveryModeChanged, true, true, true)]
    [InlineData(ExchangeName.InvoiceDeliveryModeChanged, false, false, true)]
    [InlineData(ExchangeName.InvoiceDeliveryModeChanged, false, true, false)]
    [InlineData(ExchangeName.InvoiceDeliveryModeChanged, true, false, false)]
    [InlineData(ExchangeName.InvoiceDeliveryModeChanged, false, true, true)]
    [InlineData(ExchangeName.InvoiceDeliveryModeChanged, true, true, false)]
    [InlineData(ExchangeName.InvoiceDeliveryModeChanged, true, false, true)]
    public void DateChangeReason_InvoiceDeliveryModeChanged_Success (string exchangeName, bool forbiddenPointInvoicePrevious, bool previousDateIsEmpty, bool datesHasBeenChanged)
    {
        // Arrange
        var reasonEstimator = new DateChangeReasonEstimator();
        var command = Commands.CommandsFactory(exchangeName);
        var invoice = Invoices.InvoiceFactoryRedirectionTests(forbiddenPointInvoicePrevious ? "0001-001" : "1111-111");
        var invoice2 = Invoices.InvoiceFactoryRedirectionTests(forbiddenPointInvoicePrevious ? "0001-001" : "2222-222");
        var current = Estimates.EstimateFactory(invoice, false, previousDateIsEmpty, datesHasBeenChanged);
        var previous = Estimates.EstimateFactory(invoice2, true, previousDateIsEmpty, datesHasBeenChanged);
        
        // Act
        var result =  reasonEstimator.Estimate(command, invoice, current, previous);

        // Assert
        Assert.Equal(DateChangeReasonType.None, result.Value.Value);
    }
    
    [Theory]
    [InlineData(ExchangeName.InvoiceSenderCityChanged, false, false, true)]
    public void DateChangeReason_InvoiceSenderCityChanged_Success (string exchangeName, bool redirectedFromForbidden, bool previousDateIsEmpty, bool datesHasBeenChanged)
    {
        // Arrange
        var reasonEstimator = new DateChangeReasonEstimator();
        var command = Commands.CommandsFactory(exchangeName);
        var invoice = Invoices.InvoiceFactoryRedirectionTests(redirectedFromForbidden ? "0001-001" : "3333-333", false, true);
        var invoice2 = Invoices.InvoiceFactoryRedirectionTests("2222-222", true);
        var current = Estimates.EstimateFactory(invoice, false, previousDateIsEmpty, datesHasBeenChanged);
        var previous = Estimates.EstimateFactory(invoice2, true, previousDateIsEmpty, datesHasBeenChanged);
        
        // Act
        var result = reasonEstimator.Estimate(command, invoice2, current, previous);
    
        // Assert
        Assert.Equal(DateChangeReasonType.SenderCityChanged, result.Value.Value);
    }
    
    [Theory]
    [InlineData(ExchangeName.InvoiceRedirected, false, true)]
    public void DateChangeReason_RecipientCityChanged_Success (string exchangeName, bool previousDateIsEmpty, bool datesHasBeenChanged)
    {
        // Arrange
        var reasonEstimator = new DateChangeReasonEstimator();
        var command = Commands.CommandsFactory(exchangeName);
        var invoice = Invoices.InvoiceFactoryRedirectionTests("2222-222");
        var invoice2 = Invoices.InvoiceFactoryRedirectionTests("3333-333", true);
        var current = Estimates.EstimateFactory(invoice2, false, previousDateIsEmpty, datesHasBeenChanged);
        var previous = Estimates.EstimateFactory(invoice, true, previousDateIsEmpty, datesHasBeenChanged);
        
        // Act
        var result = reasonEstimator.Estimate(command, invoice, current, previous);
    
        // Assert
        Assert.Equal(DateChangeReasonType.RecipientCityChanged, result.Value.Value);
    }
    
    [Theory] 
    [InlineData(ExchangeName.InvoiceRedirected, false, true)]
    public void DateChangeReason_RedirectedNotFromForbiddenPoint_Success (string exchangeName, bool previousDateIsEmpty, bool datesHasBeenChanged)
    {
        // Arrange
        var reasonEstimator = new DateChangeReasonEstimator();
        var command = Commands.CommandsFactory(exchangeName);
        var invoice = Invoices.InvoiceFactoryRedirectionTests("1111-222");
        var invoice2 = Invoices.InvoiceFactoryRedirectionTests("3333-333");
        var current = Estimates.EstimateFactory(invoice2, false, previousDateIsEmpty, datesHasBeenChanged);
        var previous = Estimates.EstimateFactory(invoice,true, previousDateIsEmpty, datesHasBeenChanged);
        
        // Act
        var result =  reasonEstimator.Estimate(command, invoice, current, previous);
    
        // Assert
        Assert.Equal(DateChangeReasonType.Forwarded, result.Value.Value);
    }
    
    [Theory]
    [InlineData(ExchangeName.InvoiceRedirected, true, true)]
    public void DateChangeReason_RedirectedFromForbiddenPoint_Success (string exchangeName, bool previousDateIsEmpty, bool datesHasBeenChanged)
    {
        // Arrange
        var reasonEstimator = new DateChangeReasonEstimator();
        var command = Commands.CommandsFactory(exchangeName);
        var invoice = Invoices.InvoiceFactoryRedirectionTests("0001-001");
        var invoice2 = Invoices.InvoiceFactoryRedirectionTests("1111-111");
        var current = Estimates.EstimateFactory(invoice, false, previousDateIsEmpty, datesHasBeenChanged);
        var previous = Estimates.EstimateFactory(invoice2, true, previousDateIsEmpty, datesHasBeenChanged);
        
        // Act
        var result =  reasonEstimator.Estimate(command, invoice2, current, previous);
    
        // Assert
        Assert.Equal(DateChangeReasonType.Forwarded, result.Value.Value);
    }
    
    [Theory]
    [InlineData(ExchangeName.DayChangedForNotAcceptedInvoiceEstimation, false, true)]
    public void ItemsNotProvidedByClientSuccessInline (string exchangeName, bool previousDateIsEmpty, bool datesHasBeenChanged)
    {
        // Arrange
        var reasonEstimator = new DateChangeReasonEstimator();
        var command = Commands.CommandsFactory(exchangeName);
        var invoice = Invoices.InvoiceFactoryRedirectionTests("3333-333");
        var invoice2 = Invoices.InvoiceFactoryRedirectionTests("3333-333");
        var current = Estimates.EstimateFactory(invoice, false, previousDateIsEmpty, datesHasBeenChanged);
        var previous = Estimates.EstimateFactory(invoice2, true, previousDateIsEmpty, datesHasBeenChanged);
        
        // Act
        var result =  reasonEstimator.Estimate(command, invoice, current, previous);
    
        // Assert
        Assert.Equal(DateChangeReasonType.ItemsNotProvidedByClient, result.Value.Value);
    }
   
    [Theory]
    [InlineData(ExchangeName.InvoiceRedirected, false, false)]
    public void DateChangeReason_NoneReason_Success (string exchangeName, bool previousDateIsEmpty, bool datesHasBeenChanged)
    {
        // Arrange
        var reasonEstimator = new DateChangeReasonEstimator();
        var command = Commands.CommandsFactory(exchangeName);
        var invoice = Invoices.InvoiceFactoryRedirectionTests("0001-001");
        var invoice2 = Invoices.InvoiceFactoryRedirectionTests("2222-222");
        var current = Estimates.EstimateFactory(invoice, false, previousDateIsEmpty, datesHasBeenChanged);
        var previous = Estimates.EstimateFactory(invoice2, true, previousDateIsEmpty, datesHasBeenChanged);
        
        //Act
        var result =  reasonEstimator.Estimate(command, invoice, current, previous);
    
        //Assert
        Assert.Equal(DateChangeReasonType.None, result.Value.Value);
    }
}