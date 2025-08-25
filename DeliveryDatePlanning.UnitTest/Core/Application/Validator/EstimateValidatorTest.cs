using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Application.Core.Validator;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Enclose;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;
using DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;
using DeliveryDatePlanning.Domain.Enums;
using Models;
using System.Reflection.Metadata;

namespace DeliveryDatePlanning.UnitTest.Core.Application.Validator;

public class EstimateValidatorTest
{
    [Theory]
    // isValid false - new estimate result equals previous estimate result
    [InlineData(true)]
    public void IsValid_Failure(bool isEqual)
    {
        // arrange
        var arrange = ArrangeFactory(isEqual);
        var validator = new EstimateValidator();

        // act
        var actual = validator.IsValid(arrange.Before, arrange.After);
        
        // assert
        Assert.False(actual);
    }
    
    [Theory]
    // isValid true - estimate result is success, new estimate result not equals previous estimate result
    [InlineData(false)]
    public void IsValid_Success(bool isEqual)
    {
        // arrange
        var arrange = ArrangeFactory(isEqual);
        var validator = new EstimateValidator();

        // act
        var actual = validator.IsValid(arrange.Before, arrange.After);
        
        // assert
        Assert.True(actual);
    }

    private (Estimate Before, Result<Estimate> After) ArrangeFactory(bool isEqual)
    {
        var timestamp = DateTime.Now;
        var id = "123-456";
        var dateStart = DateOnly.FromDateTime(DateTime.Now);
        var dateEnd =  DateOnly.FromDateTime(DateTime.Now);
        var deliveryDates = DeliveryDates.Create(dateStart, dateEnd).Value;
        var status = EstimateStatusTypeEnum.Create(EstimateStatusType.NotReached).Value;
        var reason = DateChangeReasonTypeEnum.Create(DateChangeReasonType.None).Value;
        var expiration = OverdueDays.Create(5).Value;
        var invoice = InvoiceFactory(timestamp);
        
        var before = Estimate.Create(id, deliveryDates, status, reason, expiration, invoice);

        if (isEqual)
        {
            var id1 = "123-456";
           
            var status1 = EstimateStatusTypeEnum.Create(EstimateStatusType.NotReached).Value;
            var reason1 = DateChangeReasonTypeEnum.Create(DateChangeReasonType.None).Value;
            var expiration1 = OverdueDays.Create(5).Value;
            var invoice1 = InvoiceFactory(timestamp);

            var after1 = Estimate.Create(id1, deliveryDates, status1, reason1, expiration1, invoice1);
            
            return (before.Value, after1);
        }
        
        var id2 = "123-456";
       
        var status2 = EstimateStatusTypeEnum.Create(EstimateStatusType.Expired).Value;
        var reason2 = DateChangeReasonTypeEnum.Create(DateChangeReasonType.None).Value;
        var expiration2 = OverdueDays.Create(10).Value;
        var invoice2 = InvoiceFactory(timestamp);
        
        var after2 = Estimate.Create(id2, deliveryDates, status2, reason2, expiration2, invoice2);
            
        return (before.Value, after2);
    }
    
    private Invoice InvoiceFactory(DateTime timestamp)
    {
        var postageType = PostageTypeEnum.Create(PostageType.BC2C).Value;
        var gettingType = GettingTypeEnum.Create(GettingType.OnCall).Value;
        var recipientNumber = DeliveryPointNumber.Create($"123{timestamp.Millisecond}-456").Value;
        var recipientName = DeliveryPointJurName.Create("Some name").Value;
        var contract = ContractNumber.Create("9876543210").Value;
        var senderNumber = DeliveryPointNumber.Create($"123{timestamp.Millisecond}-456").Value;
        var senderName = DeliveryPointJurName.Create("Some name").Value;
        var cityTo = CityId.Create("123-456").Value;
        var cityFrom = CityId.Create("123-456").Value;
        var deliveryMode = DeliveryMode.Create(1).Value;
        var receptionDate = ReceptionDate.Create(timestamp).Value;

        var encloses = new Enclose[2];

        for (var i = 0; i < encloses.Length; i++)
        {
            var encloseState = EncloseStateEnum.Create(EncloseState.Received).Value;
            var history = new[] { EncloseStateHistoryRecord.Create(timestamp, EncloseState.Received).Value };
            
            encloses[i] = Enclose.Create($"123{timestamp.Millisecond}-456", encloseState, history).Value;
        }
        
        var linkedInvoice = LinkedInvoice.Create("123-456", postageType, encloses, recipientName).Value;

        var isUtilization = IsUtilization.Create(false).Value;
        var isSendToReturn = IsSendToReturn.Create(false).Value;

        var result = Invoice.Create($"123{timestamp.Millisecond}-456", postageType, gettingType, recipientNumber, recipientName, contract, senderNumber, senderName, cityTo, cityFrom, deliveryMode, receptionDate, encloses, linkedInvoice, isUtilization, isSendToReturn);

        return result.Value;
    }
}