using AutoMapper;
using DeliveryDatePlanning.Application.Common.Exception;
using DeliveryDatePlanning.Application.Common.Mapping;
using DeliveryDatePlanning.Application.Declaration.Store;
using DeliveryDatePlanning.Data.Context;
using DeliveryDatePlanning.Data.Store;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Enclose;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;
using DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;
using DeliveryDatePlanning.Domain.Enums;
using DeliveryDatePlanning.UnitTest.Common;
using Models;
using System.Reflection.Metadata;

namespace DeliveryDatePlanning.UnitTest.Data;

public class EstimateStoreTest : IDisposable
{
    private readonly EstimateDbContext context;
    private readonly IEstimateStore store;

    public EstimateStoreTest()
    {
        var configurationProvider = new MapperConfiguration(cfg =>
        {
            cfg.AddProfile(new AssemblyMappingProfile(typeof(IEstimateStore).Assembly));
        });
        
        this.context = EstimateContextFactory.Create();
        this.store = new EstimateStore(this.context, configurationProvider.CreateMapper());
    }

    // findOrDefault return default, entity is not found
    [Theory(Skip = "need mongodb mock")]
    [InlineData(EstimateContextFactory.InvoiceKeyNotFound, null)]
    [InlineData(null, EstimateContextFactory.EncloseKeyNotFound)]
    public async Task FindOrDefault_EntityNotFound_ReturnDefault(string invoiceKey, string encloseKey)
    {
        // arrange
        var filter = new EstimateFilter(invoiceKey, encloseKey);
        
        // act
        var actual = await this.store.FindOrDefault(filter);

        // assert
        Assert.Null(actual);
    }
    
    // findOrDefault throw entity is not valid
    [Theory(Skip = "need mongodb mock")]
    [InlineData(EstimateContextFactory.InvoiceKeyIsFailure, null)]
    [InlineData(null, EstimateContextFactory.EncloseKeyIsFailure)]
    public async Task FindOrDefault_EntityNotValid_ThrowException(string invoiceKey, string encloseKey)
    {
        // arrange
        var filter = new EstimateFilter(invoiceKey, encloseKey);

        // act
        // assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await this.store.FindOrDefault(filter));
    }
    
    // findOrDefault success by invoice key
    [Theory(Skip = "need mongodb mock")]
    [InlineData(EstimateContextFactory.InvoiceKey, null)]
    [InlineData(EstimateContextFactory.InvoiceKey, EstimateContextFactory.EncloseKeyNotFound)]
    public async Task FindOrDefault_ByInvoiceKey_Success(string invoiceKey, string encloseKey)
    {
        // arrange
        var filter = new EstimateFilter(invoiceKey, encloseKey);
        
        // act
        var actual = await this.store.FindOrDefault(filter);

        // assert
        Assert.True(actual.Invoice.Id == invoiceKey);
    }
    
    // findOrDefault success by enclose key
    [Theory(Skip = "need mongodb mock")]
    [InlineData(null, EstimateContextFactory.EncloseKey)]
    public async Task FindOrDefault_ByEncloseKey_Success(string invoiceKey, string encloseKey)
    {
        // arrange
        var filter = new EstimateFilter(invoiceKey, encloseKey);
        
        // act
        var actual = await this.store.FindOrDefault(filter);

        // assert
        Assert.True(actual.Id == encloseKey);
        Assert.Contains(actual.Invoice.Encloses, item => item.Id == encloseKey);
    }

    // upsert throw argument null (entity is null)
    [Fact(Skip = "need mongodb mock")]
    public async Task Upsert_NullArgument_ThrowException()
    {
        // arrange
        // act
        // assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await this.store.Upsert(null));
    }
    
    // upsert success create new entity
    // upsert success update an existing entity
    [Theory(Skip = "need mongodb mock")]
    [InlineData(false)]
    [InlineData(true)]
    public async Task Upsert_Success(bool existing)
    {
        // arrange
        var expected = EstimateFactory(existing);

        // act
        await this.store.Upsert(expected);
        var actual = await this.store.FindOrDefault(new EstimateFilter(null, expected.Id));

        // assert
        Assert.NotNull(actual);
        Assert.Same(expected, actual);
    }

    private Estimate EstimateFactory(bool existing = false)
    {
        var key = existing ? EstimateContextFactory.KeyForUpdate : "3141-592";
        
        var deliveryDates = DeliveryDates.Create(DateOnly.FromDateTime(DateTime.Now), DateOnly.FromDateTime(DateTime.Now)).Value;
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
        var contract = ContractNumber.Create("123456789").Value;
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
    
    public void Dispose()
    {
        EstimateContextFactory.Destroy(this.context);
    }
}