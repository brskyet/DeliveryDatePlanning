using System.Globalization;
using System.Reflection.Metadata;
using AutoMapper;
using DeliveryDatePlanning.Application.Common.Mapping;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Enclose;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;
using DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;
using DeliveryDatePlanning.Domain.Enums;
using DeliveryDatePlanning.UnitTest.Common;
using Models;

namespace DeliveryDatePlanning.UnitTest.Data.EstimateDb;

public class EstimateTest
{
    private readonly IMapper mapper;
    
    public EstimateTest()
    {
        var configurationProvider = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AssemblyMappingProfile(typeof(DeliveryDatePlanning.Data.Model.EstimateDb.Estimate).Assembly));
        });

        this.mapper = configurationProvider.CreateMapper();
    }
    
    [Fact]
    public void DomainEntity_MapWithDbModel_Success()
    {
        // arrange
        var entity = EstimateFactory(true);

        // act
        var document = this.mapper.Map<DeliveryDatePlanning.Data.Model.EstimateDb.Estimate>(entity);
        
        // assert
        Assert.Equal(document.Id, entity.Id);
        Assert.Equal(DateTime.TryParseExact(document.DateStart, 
                new []{"dd.MM.yyyy"}, 
                CultureInfo.InvariantCulture, DateTimeStyles.None,
                out var dateStartValue) 
            ? dateStartValue.ToString("dd.MM.yyyy") 
            : null, 
            entity.DeliveryDates.DateStart.Value.ToString("dd.MM.yyyy"));
        Assert.Equal(DateTime.TryParseExact(document.DateEnd, 
                new []{"dd.MM.yyyy"},
                CultureInfo.InvariantCulture,
                DateTimeStyles.None, 
                out var dateEndValue) 
            ? dateEndValue.ToString("dd.MM.yyyy") 
            : null,
            entity.DeliveryDates.DateEnd.Value.ToString("dd.MM.yyyy"));
        Assert.Equal(document.Status, entity.Status);
        Assert.Equal(document.Reason, entity.Reason);
        Assert.Equal(document.Overdue, entity.Overdue);
        Assert.Equal(document.Invoice.Key, entity.Invoice.Id);
        Assert.Equal(document.Invoice.PostageType, entity.Invoice.PostageType);
        Assert.Equal(document.Invoice.GettingType, entity.Invoice.GettingType);
        Assert.Equal(document.Invoice.RecipientPointNumber, entity.Invoice.RecipientPointNumber);
        Assert.Equal(document.Invoice.RecipientPointJurName, entity.Invoice.RecipientPointJurName);
        Assert.Equal(document.Invoice.Ikn, entity.Invoice.Ikn);
        Assert.Equal(document.Invoice.SenderPointNumber, entity.Invoice.SenderPointNumber);
        Assert.Equal(document.Invoice.SenderPointJurName, entity.Invoice.SenderPointJurName);
        Assert.Equal(document.Invoice.CityTo, entity.Invoice.CityTo);
        Assert.Equal(document.Invoice.CityFrom, entity.Invoice.CityFrom);
        Assert.Equal(document.Invoice.DeliveryMode, entity.Invoice.DeliveryMode);
        Assert.Equal(DateTime.Parse(document.Invoice.ReceptionDate), entity.Invoice.ReceptionDate);
        Assert.Equal(document.Invoice.LinkedInvoice?.Key, entity.Invoice.LinkedInvoice?.Id);
        Assert.Equal(document.Invoice.Encloses.Count, entity.Invoice.Encloses.Count);
    }
    
    private Estimate EstimateFactory(bool existing = false)
    {
        var key = existing ? EstimateContextFactory.KeyForUpdate : "3141-592";
        
        var deliveryDates = DeliveryDates.Create(DateOnly.FromDateTime(DateTime.Now),DateOnly.FromDateTime(DateTime.Now)).Value;
        var status = EstimateStatusTypeEnum.Create(EstimateStatusType.NotReached).Value;
        var reason = DateChangeReasonTypeEnum.Create(DateChangeReasonType.None).Value;
        var expiration = OverdueDays.Create(5).Value;
        var invoice = InvoiceFactory();
        
        var result = Estimate.Create(key, deliveryDates, status, reason, expiration, invoice);
        
        return result.Value;
    }
    
    private Invoice InvoiceFactory()
    {
        var postageType = PostageTypeEnum.Create(PostageType.BC2C).Value;
        var gettingType = GettingTypeEnum.Create(GettingType.OnCall).Value;
        var recipientNumber = DeliveryPointNumber.Create("123-456").Value;
        var recipientName = DeliveryPointJurName.Create("Some name").Value;
        var contract = ContractNumber.Create("9000000001").Value;
        var senderNumber = DeliveryPointNumber.Create("123-456").Value;
        var senderName = DeliveryPointJurName.Create("Some name").Value;
        var cityTo = CityId.Create("123-456").Value;
        var cityFrom = CityId.Create("123-456").Value;
        var deliveryMode = DeliveryMode.Create(1).Value;
        var receptionDate = ReceptionDate.Create(DateTime.Now).Value;
        
        var encloseState = EncloseStateEnum.Create(EncloseState.Received).Value;
        var history = new[] { EncloseStateHistoryRecord.Create(DateTime.Now, EncloseState.Received).Value };
        var encloses = new [] { Enclose.Create("123123-456", encloseState, history).Value };

        var linkedInvoice = LinkedInvoice.Create("123-456", postageType, encloses, recipientName).Value;

        var isUtilization = IsUtilization.Create(false).Value;
        var isSendToReturn = IsSendToReturn.Create(false).Value;

        var result = Invoice.Create("123-456", postageType, gettingType, recipientNumber, recipientName, contract, senderNumber, senderName, cityTo, cityFrom, deliveryMode, receptionDate, encloses, linkedInvoice, isUtilization, isSendToReturn);

        return result.Value;
    }
}