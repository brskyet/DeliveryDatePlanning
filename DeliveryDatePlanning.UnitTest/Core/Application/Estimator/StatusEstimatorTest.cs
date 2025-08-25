using DeliveryDatePlanning.Application.Core.Estimator;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Enclose;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;
using DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;
using DeliveryDatePlanning.Domain.Enums;
using Models;

namespace DeliveryDatePlanning.UnitTest.Core.Application.Estimator;

public class StatusEstimatorTest
{
    [Theory]
    [MemberData(nameof(EstimateNotReachedIsSuccessData), false, false)]
    public void Estimate_NotReached_IsSuccess(int state, bool setUtilization, bool setSendToReturn)
    {
        // arrange
        var invoice = InvoiceFactory((EncloseState) state, setUtilization, setSendToReturn);
        var current = EstimateFactory(invoice, DateOnly.Parse("2022-01-15"), DateOnly.Parse("2022-01-20"), EstimateStatusType.None);
        var estimator = new StatusEstimator();

        // act
        var result = estimator.Estimate(DateOnly.Parse("2022-01-10"), current);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EstimateStatusType.NotReached, result.Value);
    }

    [Theory]
    [InlineData(EncloseState.DeliveredToPT)]
    [InlineData(EncloseState.ReturnedToClient)]
    [InlineData(EncloseState.Received)]
    public void Estimate_NotReachedDelivered_IsSuccess(EncloseState state)
    {
        // arrange
        var invoice = InvoiceFactory(state);
        var current = EstimateFactory(invoice, DateOnly.Parse("2022-01-15"), DateOnly.Parse("2022-01-20"), EstimateStatusType.NotReached);
        var estimator = new StatusEstimator();

        // act
        var result = estimator.Estimate(DateOnly.Parse("2022-01-10"), current);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EstimateStatusType.NotReachedDelivered, result.Value);
    }
    
    [Theory]
    [InlineData(EncloseState.Rejected, false, false)]
    [InlineData(EncloseState.Annulled, false, false)]
    [InlineData(EncloseState.Registered, true, false)]
    [InlineData(EncloseState.Registered, false, true)]
    public void Estimate_NotReachedRejected_IsSuccess(EncloseState state, bool setUtilization, bool setSendToReturn)
    {
        // arrange
        var invoice = InvoiceFactory(state, setUtilization, setSendToReturn);
        var current = EstimateFactory(invoice, DateOnly.Parse("2022-01-15"), DateOnly.Parse("2022-01-20"), EstimateStatusType.NotReached);
        var estimator = new StatusEstimator();

        // act
        var result = estimator.Estimate(DateOnly.Parse("2022-01-10"), current);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EstimateStatusType.NotReachedRejected, result.Value);
    }
    
    [Theory]
    [MemberData(nameof(EstimateReachedIsSuccessData))]
    public void Estimate_Reached_IsSuccess(string now, int state)
    {
        // arrange
        var invoice = InvoiceFactory((EncloseState) state);
        var current = EstimateFactory(invoice, DateOnly.Parse("2022-01-15"), DateOnly.Parse("2022-01-20"), EstimateStatusType.NotReached);
        var estimator = new StatusEstimator();

        // act
        var result = estimator.Estimate(DateOnly.Parse(now), current);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EstimateStatusType.Reached, result.Value);
    }
    
    [Theory]
    [MemberData(nameof(EstimateExpiredIsSuccessData))]
    public void Estimate_Expired_IsSuccess(int status, int state)
    {
        // arrange
        var invoice = InvoiceFactory((EncloseState) state);
        var current = EstimateFactory(invoice, DateOnly.Parse("2022-01-15"), DateOnly.Parse("2022-01-20"), (EstimateStatusType) status);
        var estimator = new StatusEstimator();

        // act
        var result = estimator.Estimate(DateOnly.Parse("2022-01-21"), current);

        // assert
        Assert.True(result.IsSuccess);
        Assert.Equal(EstimateStatusType.Expired, result.Value);
    }

    [Theory]
    [MemberData(nameof(EstimateExpiredIsSuccessData))]
    public void Estimate_Expired_IsNot(int status, int state)
    {
        // arrange
        var invoice = InvoiceFactory((EncloseState) state);
        var current = EstimateFactory(invoice, DateOnly.Parse("2022-01-15"), DateOnly.Parse("2022-01-20"), (EstimateStatusType) status);
        var estimator = new StatusEstimator();

        // act
        var result = estimator.Estimate(DateOnly.Parse("2022-01-20"), current);

        // assert
        Assert.True(result.IsSuccess);
        Assert.NotEqual(EstimateStatusType.Expired, result.Value);
    }

    [Fact]
    public void Estimate_EstimateIsNull_IsFailure()
    {
        // arrange
        var estimator = new StatusEstimator();
        
        // act
        var result = estimator.Estimate(DateOnly.Parse("2022-01-21"), null);
        
        // assert
        Assert.True(result.IsFailure);
    }
    
    [Fact]
    public void Estimate_IsDeliveryDateEmpty_CurrentStatusUnchanged()
    {
        // arrange
        var invoice = InvoiceFactory(EncloseState.Received);
        var current = EstimateFactory(invoice, null, null, EstimateStatusType.NotReached);
        var estimator = new StatusEstimator();
        
        // act
        var result = estimator.Estimate(DateOnly.Parse("2022-01-21"), current);
        
        // assert
        Assert.True(result.IsSuccess);
        Assert.Equal(current.Status, result.Value);
    }

    private Invoice InvoiceFactory(EncloseState state, bool setUtilization = false, bool setSendToReturn = false)
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

        var isUtilization = IsUtilization.Create(setUtilization).Value;
        var isSendToReturn = IsSendToReturn.Create(setSendToReturn).Value;
        
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


    public static IEnumerable<object[]> EstimateNotReachedIsSuccessData(bool isUtilization, bool isSendToReturn) =>
        new List<object[]>
        {
            new object[] {100, isUtilization, isSendToReturn}, new object[] {101, isUtilization, isSendToReturn}, new object[] {102, isUtilization, isSendToReturn}, new object[] {103, isUtilization, isSendToReturn}, new object[] {104, isUtilization, isSendToReturn},
            new object[] {105, isUtilization, isSendToReturn}, new object[] {106, isUtilization, isSendToReturn}, new object[] {107, isUtilization, isSendToReturn}, new object[] {108, isUtilization, isSendToReturn}, new object[] {110, isUtilization, isSendToReturn},
            new object[] {112, isUtilization, isSendToReturn}, new object[] {115, isUtilization, isSendToReturn}, new object[] {116, isUtilization, isSendToReturn}, new object[] {117, isUtilization, isSendToReturn}, new object[] {118, isUtilization, isSendToReturn},
            new object[] {119, isUtilization, isSendToReturn}, new object[] {120, isUtilization, isSendToReturn}, new object[] {121, isUtilization, isSendToReturn}, new object[] {122, isUtilization, isSendToReturn}, new object[] {123, isUtilization, isSendToReturn},
            new object[] {124, isUtilization, isSendToReturn}, new object[] {125, isUtilization, isSendToReturn}, new object[] {126, isUtilization, isSendToReturn}, new object[] {127, isUtilization, isSendToReturn}, new object[] {129, isUtilization, isSendToReturn},
            new object[] {130, isUtilization, isSendToReturn}, new object[] {131, isUtilization, isSendToReturn}, new object[] {132, isUtilization, isSendToReturn}, new object[] {133, isUtilization, isSendToReturn}, new object[] {134, isUtilization, isSendToReturn},
            new object[] {135, isUtilization, isSendToReturn}, new object[] {136, isUtilization, isSendToReturn}, new object[] {137, isUtilization, isSendToReturn}, new object[] {138, isUtilization, isSendToReturn}, new object[] {139, isUtilization, isSendToReturn},
            new object[] {140, isUtilization, isSendToReturn}, new object[] {141, isUtilization, isSendToReturn}, new object[] {142, isUtilization, isSendToReturn}, new object[] {143, isUtilization, isSendToReturn}, new object[] {144, isUtilization, isSendToReturn},
            new object[] {145, isUtilization, isSendToReturn}, new object[] {146, isUtilization, isSendToReturn}, new object[] {147, isUtilization, isSendToReturn}, new object[] {148, isUtilization, isSendToReturn}, new object[] {149, isUtilization, isSendToReturn},
            new object[] {150, isUtilization, isSendToReturn}, new object[] {151, isUtilization, isSendToReturn}, new object[] {152, isUtilization, isSendToReturn}, new object[] {153, isUtilization, isSendToReturn}, new object[] {154, isUtilization, isSendToReturn},
            new object[] {155, isUtilization, isSendToReturn},
        };

    public static IEnumerable<object[]> EstimateReachedIsSuccessData =>
        new List<object[]>
        {
            new object[] {"2022-01-15", 100}, new object[] {"2022-01-15", 101}, new object[] {"2022-01-15", 102},
            new object[] {"2022-01-15", 105}, new object[] {"2022-01-15", 106},
            new object[] {"2022-01-15", 107}, new object[] {"2022-01-15", 108}, new object[] {"2022-01-15", 118},
            new object[] {"2022-01-15", 119}, new object[] {"2022-01-15", 120},
            new object[] {"2022-01-15", 121}, new object[] {"2022-01-15", 123}, new object[] {"2022-01-15", 137},
            new object[] {"2022-01-15", 138}, new object[] {"2022-01-15", 143},
            new object[] {"2022-01-15", 144}, new object[] {"2022-01-15", 145}, new object[] {"2022-01-15", 146},
            new object[] {"2022-01-15", 147}, new object[] {"2022-01-15", 153},

            new object[] {"2022-01-17", 100}, new object[] {"2022-01-17", 101}, new object[] {"2022-01-17", 102},
            new object[] {"2022-01-17", 105}, new object[] {"2022-01-17", 106},
            new object[] {"2022-01-17", 107}, new object[] {"2022-01-17", 108}, new object[] {"2022-01-17", 118},
            new object[] {"2022-01-17", 119}, new object[] {"2022-01-17", 120},
            new object[] {"2022-01-17", 121}, new object[] {"2022-01-17", 123}, new object[] {"2022-01-17", 137},
            new object[] {"2022-01-17", 138}, new object[] {"2022-01-17", 143},
            new object[] {"2022-01-17", 144}, new object[] {"2022-01-17", 145}, new object[] {"2022-01-17", 146},
            new object[] {"2022-01-17", 147}, new object[] {"2022-01-17", 153},

            new object[] {"2022-01-20", 100}, new object[] {"2022-01-20", 101}, new object[] {"2022-01-20", 102},
            new object[] {"2022-01-20", 105}, new object[] {"2022-01-20", 106},
            new object[] {"2022-01-20", 107}, new object[] {"2022-01-20", 108}, new object[] {"2022-01-20", 118},
            new object[] {"2022-01-20", 119}, new object[] {"2022-01-20", 120},
            new object[] {"2022-01-20", 121}, new object[] {"2022-01-20", 123}, new object[] {"2022-01-20", 137},
            new object[] {"2022-01-20", 138}, new object[] {"2022-01-20", 143},
            new object[] {"2022-01-20", 144}, new object[] {"2022-01-20", 145}, new object[] {"2022-01-20", 146},
            new object[] {"2022-01-20", 147}, new object[] {"2022-01-20", 153},
        };

    public static IEnumerable<object[]> EstimateExpiredIsSuccessData =>
        new List<object[]>
        {
            new object[] { 1, 100 }, new object[] { 1, 101 }, new object[] { 1, 102 }, new object[] { 1, 105 }, new object[] { 1, 106 },  
            new object[] { 1, 107 }, new object[] { 1, 108 }, new object[] { 1, 118 }, new object[] { 1, 119 }, new object[] { 1, 120 },  
            new object[] { 1, 121 }, new object[] { 1, 123 }, new object[] { 1, 137 }, new object[] { 1, 138 }, new object[] { 1, 143 },  
            new object[] { 1, 144 }, new object[] { 1, 145 }, new object[] { 1, 146 }, new object[] { 1, 147 }, new object[] { 1, 153 }, 
    
            new object[] { 4, 100 }, new object[] { 4, 101 }, new object[] { 4, 102 }, new object[] { 4, 105 }, new object[] { 4, 106 },  
            new object[] { 4, 107 }, new object[] { 4, 108 }, new object[] { 4, 118 }, new object[] { 4, 119 }, new object[] { 4, 120 },  
            new object[] { 4, 121 }, new object[] { 4, 123 }, new object[] { 4, 137 }, new object[] { 4, 138 }, new object[] { 4, 143 },  
            new object[] { 4, 144 }, new object[] { 4, 145 }, new object[] { 4, 146 }, new object[] { 4, 147 }, new object[] { 4, 153 }, 
        };
}