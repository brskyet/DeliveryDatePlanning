using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Enclose;
using DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;
using Models;
using System.Reflection.Metadata;

namespace DeliveryDatePlanning.UnitTest.Core.Domain;

public class InvoiceTest
{
    [Theory]
    [InlineData("123-456", true, 1)]
    [InlineData("123-456", true, 10)]
    public void Create_Success(string id, bool isEnclose, int encloseLength)
    {
        // arrange, act
        var actual = InvoiceFactory(id, isEnclose, encloseLength);

        // assert
        Assert.True(actual.IsSuccess);
    }
    
    [Theory]
    [InlineData(null, true, 1)]
    [InlineData(" ", true, 1)]
    [InlineData("123-456", false, 1)]
    [InlineData("123-456", true, 0)]
    public void Create_Failure(string id, bool isEnclose, int encloseLength)
    {
        // arrange, act
        var actual = InvoiceFactory(id, isEnclose, encloseLength);

        // assert
        Assert.True(actual.IsFailure);
    }
    
    private Result<Invoice> InvoiceFactory(string id, bool isEnclose, int encloseLength)
    {
        var postageType = PostageTypeEnum.Create(PostageType.Priority).Value;
        var gettingType = GettingTypeEnum.Create(GettingType.OnCall).Value;
        var recipientNumber = DeliveryPointNumber.Create("123-456").Value;
        var recipientName = DeliveryPointJurName.Create("Some name").Value;
        var contract = ContractNumber.Create("9876543210").Value;
        var senderNumber = DeliveryPointNumber.Create("123-456").Value;
        var senderName = DeliveryPointJurName.Create("Some name").Value;
        var cityTo = CityId.Create("123-456").Value;
        var cityFrom = CityId.Create("123-456").Value;
        var deliveryMode = DeliveryMode.Create(1).Value;
        var receptionDate = ReceptionDate.Create(DateTime.Now).Value;

        var encloses = isEnclose ? new Enclose[encloseLength] : default;

        if (encloses is not null)
        {
            for (var i = 0; i < encloses.Length; i++)
            {
                var state = EncloseStateEnum.Create(EncloseState.Received).Value;
                var history = new[] { EncloseStateHistoryRecord.Create(DateTime.Now, EncloseState.Received).Value };
                
                encloses[i] = Enclose.Create("123-456", state, history).Value;
            }
        }

        var linkedInvoice = isEnclose && encloseLength > 0 ? LinkedInvoice.Create("123-456", postageType, encloses, recipientName).Value : default;

        var isUtilization = IsUtilization.Create(false).Value;
        var isSendToReturn = IsSendToReturn.Create(false).Value;

        var result = Invoice.Create(id, postageType, gettingType, recipientNumber, recipientName, contract, senderNumber, senderName, cityTo, cityFrom, deliveryMode, receptionDate, encloses, linkedInvoice, isUtilization, isSendToReturn);

        return result;
    }
}