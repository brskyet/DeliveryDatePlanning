using DeliveryDatePlanning.Application.Common.Constant;
using DeliveryDatePlanning.Application.Common.Exception;
using DeliveryDatePlanning.Application.Core.Validator;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Enclose;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;
using DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;
using DeliveryDatePlanning.Domain.Enums;
using Models;

namespace DeliveryDatePlanning.UnitTest.Core.Application.Validator;

public class CommandValidatorTest
{
    [Theory]
    // isValid false - postageTypes does not contains
    [InlineData(ExchangeName.EncloseRegistered, PostageType.ConsolidatedToPoint, true, "123-456", false, EncloseState.Received )]
    [InlineData(ExchangeName.EncloseRegistered, PostageType.ConsolidatedReturn, true, "123-456", false, EncloseState.Received )]
    [InlineData(ExchangeName.EncloseRegistered, PostageType.ConsolidatedDocsReturn, true, "123-456", false, EncloseState.Received )]
    [InlineData(ExchangeName.EncloseRegistered, PostageType.ConsolidatedResending, true, "123-456", false, EncloseState.Received )]
    [InlineData(ExchangeName.EncloseRegistered, PostageType.ConsolidatedClientReturn, true, "123-456", false, EncloseState.Received )]
    [InlineData(ExchangeName.EncloseRegistered, PostageType.Priority, true, "123-456", false, EncloseState.Received )]
    [InlineData(ExchangeName.EncloseRegistered, PostageType.PriorityWithCashOnDelivery, true, "123-456", false, EncloseState.Received )]
    [InlineData(ExchangeName.EncloseRegistered, PostageType.Abonent, true, "123-456", false, EncloseState.Received )]
    [InlineData(ExchangeName.EncloseRegistered, PostageType.ReturnPartial, true, "123-456", false, EncloseState.Received )]
    [InlineData(ExchangeName.EncloseRegistered, PostageType.ReturnPartialWithCashOnDelivery, true, "123-456", false, EncloseState.Received )]
    
    // isValid false - EncloseRegistered when estimate is not null
    [InlineData(ExchangeName.EncloseRegistered, PostageType.ToDeliveryPoint, false, "123-456", false, EncloseState.Received )]

    // isValid false - InvoiceRedirected when invoice.RecipientNumber == ForbiddenDeliveryPoint && estimate.IsDeliveryDateEmpty() == false
    [InlineData(ExchangeName.InvoiceRedirected, PostageType.ToDeliveryPoint, false, CommandValidator.ForbiddenDeliveryPoint, false, EncloseState.Received )]
    
    // isValid false - InvoiceRedirected when invoice.RecipientNumber != ForbiddenDeliveryPoint && estimate.IsDeliveryDateEmpty()
    [InlineData(ExchangeName.InvoiceRedirected, PostageType.ToDeliveryPoint, false, "123-456", true, EncloseState.Received )]
    
    // isValid false - InvoiceDeliveryModeChanged and invoice.AllEnclosesInInitialStates() == false
    [InlineData(ExchangeName.InvoiceDeliveryModeChanged, PostageType.ToDeliveryPoint, false, "123-456", false, EncloseState.Received )]
    public void IsValid_Failure(string exchange, PostageType postageType, bool isEstimateNull, string recipientNumber, bool isDeliveryDateEmpty, EncloseState encloseState)
    {
        // arrange
        var command = new EstimateCommand(DateTime.Now, exchange);
        var invoice = InvoiceFactory(postageType, recipientNumber, encloseState);
        var estimate = isEstimateNull ? null : EstimateFactory(invoice, isDeliveryDateEmpty);
        var validator = new CommandValidator();

        // act
        var actual = validator.IsValid(command, invoice, estimate);

        // assert
        Assert.False(actual);
    }

    [Theory]
    // isValid true - postageTypes contains
    [InlineData(ExchangeName.EncloseRegistered, PostageType.ToDeliveryPoint, true, "123-456", false, EncloseState.Received )]
    [InlineData(ExchangeName.EncloseRegistered, PostageType.ToDeliveryPointWithCashOnDelivery, true, "123-456", false, EncloseState.Received )]
    [InlineData(ExchangeName.EncloseRegistered, PostageType.PostReturn, true, "123-456", false, EncloseState.Received )]
    [InlineData(ExchangeName.EncloseRegistered, PostageType.BC2C, true, "123-456", false, EncloseState.Received )]
    [InlineData(ExchangeName.EncloseRegistered, PostageType.BC2CCashOnDelivery, true, "123-456", false, EncloseState.Received )]
    
    // isValid true - when InvoiceRedirected and invoice.RecipientNumber == ForbiddenDeliveryPoint && estimate.IsDeliveryDateEmpty() == true
    [InlineData(ExchangeName.InvoiceRedirected, PostageType.ToDeliveryPoint, false, CommandValidator.ForbiddenDeliveryPoint, true, EncloseState.Received )]
    
    // isValid true - when InvoiceRedirected and invoice.RecipientNumber != ForbiddenDeliveryPoint && estimate.IsDeliveryDateEmpty() == false
    [InlineData(ExchangeName.InvoiceRedirected, PostageType.ToDeliveryPoint, false, "123-456", false, EncloseState.Received )]
    
    // isValid true - when  InvoiceDeliveryModeChanged and invoice.AllEnclosesInInitialStates():
    [InlineData(ExchangeName.InvoiceDeliveryModeChanged, PostageType.ToDeliveryPoint, false, "123-456", false, EncloseState.Registered )]
    [InlineData(ExchangeName.InvoiceDeliveryModeChanged, PostageType.ToDeliveryPoint, false, "123-456", false, EncloseState.FormedForPassingToLogistician )]
    [InlineData(ExchangeName.InvoiceDeliveryModeChanged, PostageType.ToDeliveryPoint, false, "123-456", false, EncloseState.WaitingForClientDeliveryToPT )]
    [InlineData(ExchangeName.InvoiceDeliveryModeChanged, PostageType.ToDeliveryPoint, false, "123-456", false, EncloseState.ReadyForSending )]
    public void IsValid_Success(string exchange, PostageType postageType, bool isEstimateNull, string recipientNumber, bool isDeliveryDateEmpty, EncloseState encloseState)
    {
        // arrange
        var command = new EstimateCommand(DateTime.Now, exchange);
        var invoice = InvoiceFactory(postageType, recipientNumber, encloseState);
        var estimate = isEstimateNull ? null : EstimateFactory(invoice, isDeliveryDateEmpty);
        var validator = new CommandValidator();

        // act
        var actual = validator.IsValid(command, invoice, estimate);

        // assert
        Assert.True(actual);
    }

    // throws exception when InvoiceRedirected and estimate is null
    [Fact]
    public void IsValid_EstimateIsNull_ThrowException()
    {
        // arrange
        var command = new EstimateCommand(DateTime.Now, ExchangeName.InvoiceRedirected);
        var invoice = InvoiceFactory(PostageType.ToDeliveryPoint, "123-456", EncloseState.Received);
        var validator = new CommandValidator();

        // act
        // assert
        Assert.Throws<InternalException>(() => validator.IsValid(command, invoice, null));
    }

    private Invoice InvoiceFactory(PostageType postageTypeValue, string recipientNumberValue, EncloseState encloseStateValue)
    {
        var postageType = PostageTypeEnum.Create(postageTypeValue).Value;
        var gettingType = GettingTypeEnum.Create(GettingType.OnCall).Value;
        var recipientNumber = DeliveryPointNumber.Create(recipientNumberValue).Value;
        var recipientName = DeliveryPointJurName.Create("Some name").Value;
        var contract = ContractNumber.Create("9876543210").Value;
        var senderNumber = DeliveryPointNumber.Create("123-456").Value;
        var senderName = DeliveryPointJurName.Create("Some name").Value;
        var cityTo = CityId.Create("123-456").Value;
        var cityFrom = CityId.Create("123-456").Value;
        var deliveryMode = DeliveryMode.Create(1).Value;
        var receptionDate = ReceptionDate.Create(DateTime.Now).Value;

        var encloses = new Enclose[2];

        for (var i = 0; i < encloses.Length; i++)
        {
            var encloseState = EncloseStateEnum.Create(encloseStateValue).Value;
            var history = new[] { EncloseStateHistoryRecord.Create(DateTime.Now, EncloseState.Received).Value };
            
            encloses[i] = Enclose.Create($"123{i}-456", encloseState, history).Value;
        }
        
        var linkedInvoice = LinkedInvoice.Create("123-456", postageType, encloses, recipientName).Value;

        var isUtilization = IsUtilization.Create(false).Value;
        var isSendToReturn = IsSendToReturn.Create(false).Value;
       
        var result = Invoice.Create("123-456", postageType, gettingType, recipientNumber, recipientName, contract, senderNumber, senderName, cityTo, cityFrom, deliveryMode, receptionDate, encloses, linkedInvoice, isUtilization, isSendToReturn);

        return result.Value;
    }
    
    private Estimate EstimateFactory(Invoice invoice, bool isDeliveryDateEmpty)
    {
  
        DateOnly? dateStart = isDeliveryDateEmpty ? null : DateOnly.FromDateTime(DateTime.Now);
        DateOnly?  dateEnd = isDeliveryDateEmpty ? null : DateOnly.FromDateTime(DateTime.Now);
        var deliveryDates = DeliveryDates.Create(dateStart, dateEnd).Value;
        var status = EstimateStatusTypeEnum.Create(EstimateStatusType.NotReached).Value;
        var reason = DateChangeReasonTypeEnum.Create(DateChangeReasonType.None).Value;
        var expiration = OverdueDays.Create(5).Value;
        
        var result = Estimate.Create("123-456", deliveryDates, status, reason, expiration, invoice);
        
        return result.Value;
    }
}