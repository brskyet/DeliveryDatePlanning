using System.ComponentModel;

namespace DeliveryDatePlanning.Domain.Enums;
/// <summary>
/// Тарифная зона
/// </summary>
public enum TariffZoneType
{
    [Description("-")]
    None = 0,
    
    [Description("2022 General Вызов курьера + самопривоз на СЦ")]
    GeneralCourierWarehouseZone,
    
    [Description("2022 General_5Post Вызов курьера + самопривоз на СЦ")]
    GeneralFivePostCourierWarehouseZone,
    
    [Description("2022 General сдача через постаматы/пункты выдачи")]
    GeneralDeliveryPointZone,
    
    [Description("2022 General_5Post сдача через постаматы/пункты выдачи")]
    GeneralFivePostDeliveryPointZone,
    
    [Description("2022 General сдача через постаматы/пункты выдачи")]
    BC2CGeneralDeliveryPointZone,
    
    [Description("2022 General_5Post сдача через постаматы/пункты выдачи")]
    BC2CGeneralFivePostDeliveryPointZone,
    
    [Description("Сроки c2c")]
    C2CZone,
    
    [Description("2022 General возвраты")]
    GeneralReturnsZone,
    
    [Description("2022 General возвраты")]
    GeneralReturnsPostReturnZone,
    
    [Description("2022 General возвраты")]
    GeneralReturnsClientReturnZone,
    
    [Description("2022 General_5Post возвраты")]
    GeneralFivePostReturnsZone,
    
    [Description("2022 General_5Post возвраты")]
    GeneralFivePostReturnsPostReturnZone,

    [Description("2022 General_5Post возвраты")]
    GeneralFivePostReturnsClientReturnZone,

    [Description("2022 General сдача через постаматы/пункты выдачи")]
    BC2CReturnGeneralDeliveryPointZone,
    
    [Description("2022 General_5Post сдача через постаматы/пункты выдачи")]
    BC2CReturnGeneralFivePostDeliveryPointZone,
    
    [Description("2022 General возвраты")]
    BC2CReturnGeneralZone
}