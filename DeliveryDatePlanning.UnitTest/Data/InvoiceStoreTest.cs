using DeliveryDatePlanning.Application.Common.Exception;
using DeliveryDatePlanning.Application.Declaration.Store;
using DeliveryDatePlanning.Data.Context;
using DeliveryDatePlanning.Data.Store;
using DeliveryDatePlanning.UnitTest.Common;

namespace DeliveryDatePlanning.UnitTest.Data;

public class InvoiceStoreTest : IDisposable
{
    private readonly AptContext context;
    private readonly IInvoiceStore store;
    
    public InvoiceStoreTest()
    {
        this.context = InvoiceContextFactory.Create();
        this.store = new InvoiceStore(this.context);
    }

    // find throw entity not found
    [Theory]
    [InlineData(InvoiceContextFactory.InvoiceKeyNotFound, null)]
    [InlineData(null, InvoiceContextFactory.EncloseKeyNotFound)]
    public async Task Find_EntityNotFound_ThrowException(string invoiceKey, string encloseKey)
    {
        // arrange
        var filter = new InvoiceFilter(invoiceKey, encloseKey);

        // act
        // assert
        await Assert.ThrowsAsync<NotFoundException>(async () => await this.store.Find(filter));
    }
    
    // find throw entity is failure (not valid)
    [Theory]
    [InlineData(InvoiceContextFactory.InvoiceKeyIsFailure1, null)]
    [InlineData(null, InvoiceContextFactory.EncloseKeyIsFailure)]
    public async Task Find_EntityIsFailure_ThrowException(string invoiceKey, string encloseKey)
    {
        // arrange
        var filter = new InvoiceFilter(invoiceKey, encloseKey);
        
        // act
        // assert
        await Assert.ThrowsAnyAsync<Exception>(async () => await this.store.Find(filter));
    }
    
    // find success by invoice key
    [Theory]
    [InlineData(InvoiceContextFactory.InvoiceKey, null)]
    [InlineData(InvoiceContextFactory.InvoiceKey, InvoiceContextFactory.EncloseKeyNotFound)]
    public async Task Find_ByInvoiceKey_Success(string invoiceKey, string encloseKey)
    {
        // arrange
        var filter = new InvoiceFilter(invoiceKey, encloseKey);
        
        // act
        var actual = await this.store.Find(filter);

        // assert
        Assert.True(actual.Id == invoiceKey);
    }
    
    // find success by enclose key
    [Theory]
    [InlineData(null, InvoiceContextFactory.EncloseKey)]
    public async Task Find_ByEncloseKey_Success(string invoiceKey, string encloseKey)
    {
        // arrange
        var filter = new InvoiceFilter(invoiceKey, encloseKey);
        
        // act
        var actual = await this.store.Find(filter);

        // assert
        Assert.Contains(actual.Encloses, item => item.Id == encloseKey);
    }

    public void Dispose()
    {
        InvoiceContextFactory.Destroy(this.context);
    }
}