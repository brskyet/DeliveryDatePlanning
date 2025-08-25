using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Enclose;
using DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;
using Models;
using System.Reflection.Metadata;
using System.Xml.Linq;

namespace DeliveryDatePlanning.UnitTest.Common.TestDataProviders;

public static class Invoices
{
    public static Invoice InvoiceFactoryRedirectionTests(string delvieryPointNumber, bool cityToChanged = false, bool cityFromChaged = false)
    {
        var postageType = PostageTypeEnum.Create(PostageType.ToDeliveryPoint).Value;
        var gettingType = GettingTypeEnum.Create(GettingType.CourierCall).Value;
        var recipientNumber = DeliveryPointNumber.Create(delvieryPointNumber).Value;
        var recipientName = DeliveryPointJurName.Create("Not 5 post name").Value;
        var contract = ContractNumber.Create("9876543210").Value;
        var senderNumber = DeliveryPointNumber.Create("1234-567").Value;
        var senderName = DeliveryPointJurName.Create("Some name").Value;
        var cityTo = CityId.Create(cityToChanged  ? "123-456" : "772-321").Value;
        var cityFrom = CityId.Create(cityFromChaged ? "654-321" : "123-456").Value;
        var deliveryMode = DeliveryMode.Create(1).Value;
        var receptionDate = ReceptionDate.Create(DateTime.Now).Value;

        var encloses = new Enclose[1];

        for (var i = 0; i < encloses.Length; i++)
        {
            var encloseState = EncloseStateEnum.Create(101).Value;
            var history = new[] { EncloseStateHistoryRecord.Create(DateTime.Now, 101).Value };

            encloses[i] = Enclose.Create($"123{i}-456", encloseState, history).Value;
        }
        
        var linkedInvoice = LinkedInvoice.Create("123-456", postageType, encloses, recipientName).Value;
            
        var isUtilization = IsUtilization.Create(false).Value;
        var isSendToReturn = IsSendToReturn.Create(false).Value;

        var invoice = Invoice.Create("123-456", postageType, gettingType, recipientNumber, recipientName, contract,
            senderNumber, senderName, cityTo, cityFrom, deliveryMode, receptionDate, encloses, linkedInvoice, isUtilization, isSendToReturn).Value;
        
        return invoice;
    }
    
    public static Invoice InvoiceFactoryForZoneDefinition(PostageType postType, GettingType getType, bool fivePostName = false, bool ppBoxIkn = false)
    {
        var postageType = PostageTypeEnum.Create(postType).Value;
        var gettingType = GettingTypeEnum.Create(getType).Value;
        var recipientNumber = DeliveryPointNumber.Create("2222-222").Value;
        var recipientName = DeliveryPointJurName.Create(fivePostName ? "ООО \"Х5 ОМНИ\"" : "Not 5 post name").Value;
        var contract = ContractNumber.Create(ppBoxIkn ? "9990000191" : "9876543210").Value;
        var senderNumber = DeliveryPointNumber.Create("1234-567").Value;
        var senderName = DeliveryPointJurName.Create("Some name").Value;
        var cityTo = CityId.Create("123-456").Value;
        var cityFrom = CityId.Create("654-321").Value;
        var deliveryMode = DeliveryMode.Create(1).Value;
        var receptionDate = ReceptionDate.Create(DateTime.Now).Value;

        var isUtilization = IsUtilization.Create(false).Value;
        var isSendToReturn = IsSendToReturn.Create(false).Value;

        var encloses = new Enclose[1];

        for (var i = 0; i < encloses.Length; i++)
        {
            var encloseState = EncloseStateEnum.Create(101).Value;
            var history = new[] { EncloseStateHistoryRecord.Create(DateTime.Now, 101).Value };

            encloses[i] = Enclose.Create($"123{i}-456", encloseState, history).Value;
        }
        
        var linkedInvoice = LinkedInvoice.Create("123-456", postageType, encloses, recipientName).Value;
            
        var invoice = Invoice.Create("123-456", postageType, gettingType, recipientNumber, recipientName, contract,
            senderNumber, senderName, cityTo, cityFrom, deliveryMode, receptionDate, encloses, linkedInvoice, isUtilization, isSendToReturn).Value;
        
        return invoice;
    }
}