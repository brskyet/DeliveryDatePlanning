using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Enums;
using DeliveryDatePlanning.UnitTest.Common.TestDataProviders;
using Models;

namespace DeliveryDatePlanning.UnitTest.Core.Application.Estimator;

public class InvoiceTariffZoneDefinition
{
    [Theory, InlineData(PostageType.ToDeliveryPoint, GettingType.CourierCall)]
    public void DefineTariffZone_GeneralCourierWarehouseZone_Success(PostageType postageType, GettingType gettingType)
    {
        // Act
        var invoice = Invoices.InvoiceFactoryForZoneDefinition(postageType, gettingType);
        var tariffZoneType = invoice.DefineTariffZoneType();
            
        // Assert
        Assert.Equal(TariffZoneType.GeneralCourierWarehouseZone, tariffZoneType);
    }
    
    [Theory, InlineData(PostageType.ToDeliveryPoint, GettingType.CourierCall, true)]
    public void DefineTariffZone_GeneralFivePostCourierWarehouseZone_Success(PostageType postageType, GettingType gettingType, bool fivePostName)
    {
        // Act
        var invoice = Invoices.InvoiceFactoryForZoneDefinition(postageType, gettingType, fivePostName);
        var tariffZoneType = invoice.DefineTariffZoneType();
            
        // Assert
        Assert.Equal(TariffZoneType.GeneralFivePostCourierWarehouseZone, tariffZoneType);
    }
    
    [Theory, InlineData(PostageType.ToDeliveryPoint, GettingType.InDeliveryPointByPack)]
    public void DefineTariffZone_GeneralDeliveryPointZone_Success(PostageType postageType, GettingType gettingType)
    {
        // Act
        var invoice = Invoices.InvoiceFactoryForZoneDefinition(postageType, gettingType);
        var tariffZoneType = invoice.DefineTariffZoneType();
            
        // Assert
        Assert.Equal(TariffZoneType.GeneralDeliveryPointZone, tariffZoneType);
    }
    
    [Theory, InlineData(PostageType.ToDeliveryPoint, GettingType.InDeliveryPointByPack, true)]
    public void DefineTariffZone_GeneralFivePostDeliveryPointZone_Success(PostageType postageType, GettingType gettingType, bool fivePostName)
    {
        // Act
        var invoice = Invoices.InvoiceFactoryForZoneDefinition(postageType, gettingType, fivePostName);
        var tariffZoneType = invoice.DefineTariffZoneType();
            
        // Assert
        Assert.Equal(TariffZoneType.GeneralFivePostDeliveryPointZone, tariffZoneType);
    }
    
    [Theory, InlineData(PostageType.BC2C, GettingType.InDeliveryPoint)]
    public void DefineTariffZone_BC2CGeneralDeliveryPointZone_Success(PostageType postageType, GettingType gettingType)
    {
        // Act
        var invoice = Invoices.InvoiceFactoryForZoneDefinition(postageType, gettingType);
        var tariffZoneType = invoice.DefineTariffZoneType();

        // Assert
        Assert.Equal(TariffZoneType.BC2CGeneralDeliveryPointZone, tariffZoneType);
    }
    
    [Theory, InlineData(PostageType.BC2C, GettingType.InDeliveryPoint, true)]
    public void DefineTariffZone_BC2CGeneralFivePostDeliveryPointZone_Success(PostageType postageType, GettingType gettingType, bool fivePostName)
    {
        // Act
        var invoice = Invoices.InvoiceFactoryForZoneDefinition(postageType, gettingType, fivePostName);
        var tariffZoneType = invoice.DefineTariffZoneType();
            
        // Assert
        Assert.Equal(TariffZoneType.BC2CGeneralFivePostDeliveryPointZone, tariffZoneType);
    }
    
    [Theory, InlineData(PostageType.BC2C, GettingType.InDeliveryPoint, false, true)]
    public void DefineTariffZone_C2CZone_Success(PostageType postageType, GettingType gettingType, bool fivePostName, bool ppBoxikn)
    {
        // Act
        var invoice = Invoices.InvoiceFactoryForZoneDefinition(postageType, gettingType, fivePostName, ppBoxikn);
        var tariffZoneType = invoice.DefineTariffZoneType();
            
        // Assert
        Assert.Equal(TariffZoneType.C2CZone, tariffZoneType);
    }
    
    [Theory, InlineData(PostageType.Return, GettingType.CourierCall, false, true)]
    public void DefineTariffZone_GeneralReturnZone_Success(PostageType postageType, GettingType gettingType, bool fivePostName, bool ppBoxikn)
    {
        // Act
        var invoice = Invoices.InvoiceFactoryForZoneDefinition(postageType, gettingType, fivePostName, ppBoxikn);
        var tariffZoneType = invoice.DefineTariffZoneType();

        // Assert
        Assert.Equal(TariffZoneType.GeneralReturnsZone, tariffZoneType);
    }
    
    [Theory, InlineData(PostageType.Return, GettingType.CourierCall, true, true)]
    public void DefineTariffZone_GeneralFivePostReturnZone_Success(PostageType postageType, GettingType gettingType, bool fivePostName, bool ppBoxikn)
    {
        // Act
        var invoice = Invoices.InvoiceFactoryForZoneDefinition(postageType, gettingType, fivePostName, ppBoxikn);
        var tariffZoneType = invoice.DefineTariffZoneType();
            
        // Assert
        Assert.Equal(TariffZoneType.GeneralFivePostReturnsZone, tariffZoneType);
    }
    
    [Theory, InlineData(PostageType.ReturnBC2C, GettingType.OnCall, false, false)]
    public void DefineTariffZone_BC2CReturnGeneralDeliveryPointZoneZone_Success(PostageType postageType, GettingType gettingType, bool fivePostName, bool ppBoxikn)
    {
        // Act
        var invoice = Invoices.InvoiceFactoryForZoneDefinition(postageType, gettingType, fivePostName, ppBoxikn);
        var tariffZoneType = invoice.DefineTariffZoneType();
        
        // Assert
        Assert.Equal(TariffZoneType.BC2CReturnGeneralDeliveryPointZone, tariffZoneType);
    }
    
    [Theory, InlineData(PostageType.ReturnBC2C, GettingType.OnCall, true, false)]
    public void DefineTariffZone_BC2CReturnGeneralFivePostDeliveryPointZone_Success(PostageType postageType, GettingType gettingType, bool fivePostName, bool ppBoxikn)
    {
        // Act
        var invoice = Invoices.InvoiceFactoryForZoneDefinition(postageType, gettingType, fivePostName, ppBoxikn);
        var tariffZoneType = invoice.DefineTariffZoneType();
            
        // Assert
        Assert.Equal(TariffZoneType.BC2CReturnGeneralFivePostDeliveryPointZone, tariffZoneType);
    }
    
    [Theory, InlineData(PostageType.ReturnBC2C, GettingType.OnCall, false, true)]
    public void DefineTariffZone_BC2CReturnGeneralZone_Success(PostageType postageType, GettingType gettingType, bool fivePostName, bool ppBoxikn)
    {
        // Act
        var invoice = Invoices.InvoiceFactoryForZoneDefinition(postageType, gettingType, fivePostName, ppBoxikn);
        var tariffZoneType = invoice.DefineTariffZoneType();
            
        // Assert
        Assert.Equal(TariffZoneType.BC2CReturnGeneralZone, tariffZoneType);
    }
}
