using Microsoft.EntityFrameworkCore;
using DeliveryDatePlanning.Data.Context;
using DeliveryDatePlanning.Data.Model.Apt;
using Models;
using Address = DeliveryDatePlanning.Data.Model.Apt.Address;
using EncloseState = DeliveryDatePlanning.Data.Model.Apt.EncloseState;

namespace DeliveryDatePlanning.UnitTest.Common;

public class InvoiceContextFactory
{
    public const string InvoiceKeyNotFound = "111-222";
    public const string EncloseKeyNotFound = "333-444";
    public const string InvoiceKeyIsFailure1 = "555-666";
    public const string InvoiceKeyIsFailure2 = "5550-6660";
    public const string EncloseKeyIsFailure = "777-888";
    public const string InvoiceKey = "999-111";
    public const string EncloseKey = "222-333";

    public static AptContext Create()
    {
        var options = new DbContextOptionsBuilder<AptContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        var context = new AptContext(options);

        context.Database.EnsureCreated();

        context.TariffZones.AddRange(MockTariffZones());
        context.Invoices.AddRange(MockInvoices());
        context.Encloses.AddRange(MockEncloses());
        context.Contracts.AddRange(MockContracts());
        context.InvoiceDeliveryPoints.AddRange(MockInvoiceDeliveryPoints());
        context.DeliveryPoints.AddRange(MockDeliveryPoints());
        context.EncloseStates.AddRange(MockEncloseStates());
        context.Addresses.AddRange(MockAddresses());
        context.InvoiceAttributes.AddRange(MockInvoiceAttributes());
        context.DocumentTitles.AddRange(MockDocumentTitles());
        context.DocumentBodies.AddRange(MockDocumentBodies());
        context.ClientDeliveryPoints.AddRange(MockClientDeliveryPoints());
        context.Clients.AddRange(MockClients());

        context.SaveChanges();

        return context;
    }

    public static void Destroy(AptContext context)
    {
        context.Database.EnsureDeleted();
        context.Dispose();
    }

    private static TariffZone[] MockTariffZones()
    {
        return Array.Empty<TariffZone>();
    }

    private static Invoice[] MockInvoices()
    {
        return new[]
        {
            new Invoice
            {
                Id = CompanyKey.Parse(InvoiceKey).Id,
                OwnerId = CompanyKey.Parse(InvoiceKey).OwnerId,
                ContractId = 444,
                ContractOwnerId = 555,
                PostageType = PostageType.Abonent,
                GettingType = GettingType.ByNomenclature,
                OrigAddressId = 111,
                OrigAddressOwnerId = 111,
                ReceiptAddressId = 112,
                ReceiptAddressOwnerId = 111,
                ReceptionDate = DateTime.Now,
                DocumentTitleId = 100,
                DocumentTitleOwnerId = 111,
            },
            new Invoice
            {
                Id = CompanyKey.Parse(InvoiceKeyIsFailure1).Id,
                OwnerId = CompanyKey.Parse(InvoiceKeyIsFailure1).OwnerId,
                ContractId = 444,
                ContractOwnerId = 555,
                PostageType = PostageType.BC2C,
                GettingType = GettingType.InWarehouse,
                OrigAddressId = 111,
                OrigAddressOwnerId = 111,
                ReceiptAddressId = 112,
                ReceiptAddressOwnerId = 111,
                ReceptionDate = DateTime.Now,
                DocumentTitleId = 100,
                DocumentTitleOwnerId = 111,
            },
            new Invoice
            {
                Id = CompanyKey.Parse(InvoiceKeyIsFailure2).Id,
                OwnerId = CompanyKey.Parse(InvoiceKeyIsFailure2).OwnerId,
                ContractId = 444,
                ContractOwnerId = 555,
                OrigAddressId = 111,
                OrigAddressOwnerId = 111,
                ReceiptAddressId = 112,
                ReceiptAddressOwnerId = 111,
                ReceptionDate = DateTime.Now,
                DocumentTitleId = 100,
                DocumentTitleOwnerId = 111,
            },
        };
    }

    private static Enclose[] MockEncloses()
    {
        return new[]
        {
            new Enclose
            {
                Id = CompanyKey.Parse(EncloseKey).Id,
                OwnerId = CompanyKey.Parse(EncloseKey).OwnerId,
                InvoiceId = CompanyKey.Parse(InvoiceKey).Id,
                InvoiceOwnerId = CompanyKey.Parse(InvoiceKey).OwnerId,
                BarCode = "123456789",
                EncloseState = 100,
            },
            new Enclose
            {
                Id = CompanyKey.Parse(EncloseKeyIsFailure).Id,
                OwnerId = CompanyKey.Parse(EncloseKeyIsFailure).OwnerId,
                InvoiceId = CompanyKey.Parse(InvoiceKeyIsFailure2).Id,
                InvoiceOwnerId = CompanyKey.Parse(InvoiceKeyIsFailure2).OwnerId,
                BarCode = "123456789",
                EncloseState = 101,
            }
        };
    }

    private static Contract[] MockContracts()
    {
        return new[]
        {
            new Contract
            {
                Id = 444,
                OwnerId = 555,
                NumberContract = "9876543210"
            },
        };
    }

    private static InvoiceDeliveryPoint[] MockInvoiceDeliveryPoints()
    {
        return new[]
        {
            new InvoiceDeliveryPoint
            {
                Id = 555,
                OwnerId = 111,
                InvoiceId = CompanyKey.Parse(InvoiceKey).Id,
                InvoiceOwnerId = CompanyKey.Parse(InvoiceKey).OwnerId,
                DeliveryPointId = 666,
                DeliveryPointOwnerId = 111,
                FromDeliveryPointId = 666,
                FromDeliveryPointOwnerId = 222,
            },
            new InvoiceDeliveryPoint
            {
                Id = 555,
                OwnerId = 222,
                InvoiceId = CompanyKey.Parse(InvoiceKeyIsFailure1).Id,
                InvoiceOwnerId = CompanyKey.Parse(InvoiceKeyIsFailure1).OwnerId,
                DeliveryPointId = 666,
                DeliveryPointOwnerId = 222,
                FromDeliveryPointId = 666,
                FromDeliveryPointOwnerId = 111,
            },
        };
    }

    private static DeliveryPoint[] MockDeliveryPoints()
    {
        return new[]
        {
            new DeliveryPoint
            {
                Id = 666,
                OwnerId = 111,
                Number = "0001-001"
            },
            new DeliveryPoint
            {
                Id = 666,
                OwnerId = 222,
                Number = "0002-002"
            }
        };
    }


    private static EncloseState[] MockEncloseStates()
    {
        return new[]
        {
            new EncloseState
            {
                Id = 888,
                OwnerId = 111,
                EncloseId = CompanyKey.Parse(EncloseKey).Id,
                EncloseOwnerId = CompanyKey.Parse(EncloseKey).OwnerId,
                SubState = 1,
                State = Models.EncloseState.Registered,
            },
            new EncloseState
            {
                Id = 888,
                OwnerId = 222,
                EncloseId = CompanyKey.Parse(EncloseKeyIsFailure).Id,
                EncloseOwnerId = CompanyKey.Parse(EncloseKeyIsFailure).OwnerId,
                SubState = 1,
                State = Models.EncloseState.Registered,
            },
        };
    }

    private static InvoiceAttribute[] MockInvoiceAttributes()
    {
        return new[]
        {
            new InvoiceAttribute
            {
                Id = 999,
                OwnerId = 111,
                InvoiceId = CompanyKey.Parse(InvoiceKey).Id,
                InvoiceOwnerId = CompanyKey.Parse(InvoiceKey).OwnerId,
                DeliveryMode = 2
            }
        };
    }

    private static DocumentTitle[] MockDocumentTitles()
    {
        return new[]
        {
            new DocumentTitle
            {
                Id = 100,
                OwnerId = 111
            }
        };
    }

    private static DocumentBody[] MockDocumentBodies()
    {
        return new[]
        {
            new DocumentBody
            {
                Id = 101,
                OwnerId = 111,
                TitleId = 100,
                TitleOwnerId = 111,
                ObjectId = CompanyKey.Parse(InvoiceKey).Id,
                ObjectOwnerId = CompanyKey.Parse(InvoiceKey).OwnerId
            }
        };
    }

    private static Address[] MockAddresses()
    {
        return new[]
        {
            new Address
            {
                Id = 111,
                OwnerId = 111,
                CityId = 900,
                CityOwnerId = 111,
            },
            new Address
            {
                Id = 112,
                OwnerId = 111,
                CityId = 901,
                CityOwnerId = 111,
            },
        };
    }

    private static ClientDeliveryPoint[] MockClientDeliveryPoints()
    {
        return new[]
        {
            new ClientDeliveryPoint()
            {
                Id = 111,
                OwnerId = 111,
                ClientId = 222,
                ClientOwnerId = 222,
                DeliveryPointId = 333,
                DeliveryPointOwnerId = 333
            },
            new ClientDeliveryPoint()
            {
                Id = 444,
                OwnerId = 444,
                ClientId = 555,
                ClientOwnerId = 555,
                DeliveryPointId = 666,
                DeliveryPointOwnerId = 666
            }
        };
    }
    
    private static Client[] MockClients()
    {
        return new[]
        {
            new Client()
            {
                Id = 111,
                OwnerId = 222,
                CompanyName = "5 post"
            }
        };
    }
}