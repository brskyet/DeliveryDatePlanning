using DeliveryDatePlanning.Application.Core.Estimator;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Enclose;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;
using DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;
using DeliveryDatePlanning.Domain.Enums;
using Models;
using System.Reflection.Metadata;

namespace DeliveryDatePlanning.UnitTest.Core.Application.Estimator;

public class OverdueEstimatorTest
{
    [Fact]
    public void Estimate_OverdueButDateEndIsEmpty_IsFailure()
    {
        // arrange
        var invoice = InvoiceFactory(EncloseState.Received);
        var current = EstimateFactory(invoice, null, null, EstimateStatusType.Expired);
        var estimator = new OverdueEstimator();
        
        // act
        var result = estimator.Estimate(DateOnly.Parse("2022-02-02"), current);
        
        // assert
        Assert.True(result.IsFailure);
    }

    [Theory]
    [InlineData(null)]
    [InlineData(2)]
    public void Estimate_NotOverdue_CurrentSame(int? overdueValue)
    {
        // arrange
        var invoice = InvoiceFactory(EncloseState.Received);
        var current = EstimateFactory(invoice, null, null, EstimateStatusType.NotReached, overdueValue);
        var estimator = new OverdueEstimator();
        
        // act
        var result = estimator.Estimate(DateOnly.Parse("2022-02-02"), current);
        
        // assert
        Assert.True(result.IsSuccess);
        Assert.Same(current.Overdue, result.Value);
    }
    
    [Theory]
    [InlineData("2022-04-20", "2022-04-15", 5)]
    [InlineData("2020-03-02", "2020-02-28", 3)]
    public void Estimate_Overdue_IsSuccess(string now, string end, int expected)
    {
        // arrange
        var invoice = InvoiceFactory(EncloseState.Received);
        var current = EstimateFactory(invoice, DateOnly.Parse(now), DateOnly.Parse(end), EstimateStatusType.Expired);
        var estimator = new OverdueEstimator();
        
        // act
        var result = estimator.Estimate(DateOnly.Parse(now), current);
        
        // assert
        Assert.True(result.IsSuccess);
        Assert.Equal(expected, (int) result.Value);
    }

    private Invoice InvoiceFactory(EncloseState state)
    {
        var postageType = PostageTypeEnum.Create(PostageType.BC2C).Value;
        var gettingType = GettingTypeEnum.Create(GettingType.OnCall).Value;
        var recipientNumber = DeliveryPointNumber.Create("131-121").Value;
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
            var encloseState = EncloseStateEnum.Create(state).Value;
            var history = new[] { EncloseStateHistoryRecord.Create(DateTime.Now, state).Value };
                
            encloses[i] = Enclose.Create($"123{i}-456", encloseState, history).Value;
        }
        
        var linkedInvoice = LinkedInvoice.Create("123-456", postageType, encloses, recipientName).Value;

        var isUtilization = IsUtilization.Create(false).Value;
        var isSendToReturn = IsSendToReturn.Create(false).Value;

        var result = Invoice.Create("123-456", postageType, gettingType, recipientNumber, recipientName, contract, senderNumber, senderName, cityTo, cityFrom, deliveryMode, receptionDate, encloses, linkedInvoice, isUtilization, isSendToReturn);

        return result.Value;
    }
    
    private Estimate EstimateFactory(Invoice invoice, DateOnly? start, DateOnly? end, EstimateStatusType s, int? overdueValue = null)
    {
        var deliveryDates = DeliveryDates.Create(start, end).Value;
        var status = EstimateStatusTypeEnum.Create(s).Value;
        var reason = DateChangeReasonTypeEnum.Create(DateChangeReasonType.None).Value;
        var overdue = OverdueDays.Create(overdueValue).Value;
        
        var result = Estimate.Create("123-456", deliveryDates, status, reason, overdue, invoice);
        
        return result.Value;
    }
}