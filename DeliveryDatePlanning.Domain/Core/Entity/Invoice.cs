using System.Text.RegularExpressions;
using DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;
using DeliveryDatePlanning.Domain.Enums;
using Models;

namespace DeliveryDatePlanning.Domain.Core.Entity;

using CSharpFunctionalExtensions;

public class Invoice : Entity<string>
{
    /// <summary>
    /// Тип отправления
    /// </summary>
    public PostageTypeEnum PostageType { get; } // должно быть всегда заполнено
    /// <summary>
    /// Тип получения
    /// </summary>
    public GettingTypeEnum GettingType { get; } // должно быть всегда заполнено
    /// <summary>
    /// Номер ПТ получателя
    /// </summary>
    public DeliveryPointNumber RecipientPointNumber { get; } // - не для всех, модель валидна и при null value
    /// <summary>
    /// Юр. лицо постамата получателя
    /// </summary>
    public DeliveryPointJurName RecipientPointJurName { get; } // - не для всех, модель валидна и при null value
    /// <summary>
    /// ИКН
    /// </summary>
    public ContractNumber Ikn { get; } // должно быть всегда заполнено
    /// <summary>
    ///  ПТ закладки (для bc2c)
    /// </summary>
    public DeliveryPointNumber SenderPointNumber { get; } // - не обязательно, модель валидна и при null value
    /// <summary>
    /// Юр. лицо постамата отправителя
    /// </summary>
    public DeliveryPointJurName SenderPointJurName { get; } // - не обязательно, модель валидна и при null value
    /// <summary>
    /// Id города получателя
    /// </summary>
    public CityId CityTo { get; } // - должно быть всегда заполнено
    /// <summary>
    /// Id города отправителя
    /// </summary>
    public CityId CityFrom { get; } // - должно быть всегда заполнено
    /// <summary>
    /// Приоритет отправления
    /// </summary>
    public DeliveryMode DeliveryMode { get; } // - не обязательно
    /// <summary>
    /// Дата приёма
    /// </summary>
    public ReceptionDate ReceptionDate { get; private set; } // должно быть всегда заполнено
    /// <summary>
    /// Вложимые
    /// </summary>
    public IReadOnlyCollection<Enclose> Encloses { get; } // должно быть всегда заполнено, как минимум одно вложимое
    /// <summary>
    /// Связанное КО (если возврат)
    /// </summary>
    public LinkedInvoice LinkedInvoice { get; } // - не обязательно
    /// <summary>
    /// Проставлен флаг "Утилизация" таблица ExtParams_Int Id 169
    /// </summary>
    public IsUtilization IsUtilization { get; }
    /// <summary>
    /// Проставлен флаг "Отправить на возврат" таблица ExtParams_Int Id 147
    /// </summary>
    public IsSendToReturn IsSendToReturn { get; }

    private Invoice(
        string id,
        PostageTypeEnum postageType,
        GettingTypeEnum gettingType,
        DeliveryPointNumber recipientPointNumber,
        DeliveryPointJurName recipientPointName,
        ContractNumber ikn,
        DeliveryPointNumber senderNumber,
        DeliveryPointJurName senderName,
        CityId cityTo,
        CityId cityFrom,
        DeliveryMode deliveryMode,
        ReceptionDate receptionDate,
        IReadOnlyCollection<Enclose> encloses,
        LinkedInvoice linkedInvoice,
        IsUtilization isUtilization,
        IsSendToReturn isSendToReturn
    ) : base(id)
    {
        PostageType = postageType;
        GettingType = gettingType;
        RecipientPointNumber = recipientPointNumber;
        RecipientPointJurName = recipientPointName;
        Ikn = ikn;
        SenderPointNumber = senderNumber;
        SenderPointJurName = senderName;
        CityTo = cityTo;
        CityFrom = cityFrom;
        DeliveryMode = deliveryMode;
        ReceptionDate = receptionDate;
        Encloses = encloses;
        LinkedInvoice = linkedInvoice;
        IsUtilization = isUtilization;
        IsSendToReturn= isSendToReturn;
    }

    public static Result<Invoice> Create(
        string id,
        PostageTypeEnum postageType,
        GettingTypeEnum gettingType,
        DeliveryPointNumber recipientPointNumber,
        DeliveryPointJurName recipientPointName,
        ContractNumber ikn,
        DeliveryPointNumber senderNumber,
        DeliveryPointJurName senderName,
        CityId cityTo,
        CityId cityFrom,
        DeliveryMode deliveryMode,
        ReceptionDate receptionDate,
        Enclose[] encloses,
        LinkedInvoice linkedInvoice,
        IsUtilization isUtilization,
        IsSendToReturn isSendToReturn
    )
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return Result.Failure<Invoice>($"{nameof(id)} argument is null or whitespace");
        }

        if (Regex.IsMatch(id, @"(^[0-9]+)[-]([0-9]+$)") == false)
        {
            return Result.Failure<Invoice>($"{nameof(id)} format is invalid");
        }

        if (encloses is null || encloses.Length == 0)
        {
            return Result.Failure<Invoice>($"{nameof(encloses)} count must be greater than 0");
        }

        IReadOnlyCollection<PostageType> postageTypesForLinkedInvoices = new[]
        {
            Models.PostageType.Return, Models.PostageType.ReturnWithCashOnDelivery, 
            Models.PostageType.ClientReturn, Models.PostageType.ReturnBC2C,
            Models.PostageType.ReturnBC2CWithCashOnDelivery
        };
        
        if (postageTypesForLinkedInvoices.Contains(postageType) && linkedInvoice is null)
        {
            return Result.Failure<Invoice>($"{nameof(linkedInvoice)} needed for this postageType({postageType})");
        }
        
        var entity = new Invoice(id, postageType, gettingType, recipientPointNumber, recipientPointName, ikn, senderNumber, senderName, cityTo, cityFrom, deliveryMode, receptionDate, encloses, linkedInvoice, isUtilization, isSendToReturn);

        return entity;
    }
    
    
    public override bool Equals(object obj)
    {
        var entity = obj as Invoice;
        
        if ((object) entity == null)
            return false;
        
        if ((object) this == (object) entity)
            return true;

        return this.Id == entity.Id && 
               this.CityTo == entity.CityTo &&
               this.CityFrom == entity.CityFrom &&
               this.ReceptionDate == entity.ReceptionDate && 
               this.RecipientPointNumber == entity.RecipientPointNumber &&
               this.RecipientPointJurName == entity.RecipientPointJurName;
    }
    
    public Invoice SetReceptionDate(ReceptionDate value)
    {
        this.ReceptionDate = value;
        return this;
    }


    public static Result<Invoice> Create(Invoice src)
    {
        var id = src.Id;
        var postageType = PostageTypeEnum.Create(src.PostageType).Value;
        var gettingType = GettingTypeEnum.Create(src.GettingType).Value;
        var recipientPointNumber = DeliveryPointNumber.Create(src.RecipientPointNumber).Value;
        var recipientPointName = DeliveryPointJurName.Create(src.RecipientPointJurName).Value;
        var contract = ContractNumber.Create(src.Ikn).Value;
        var senderNumber = DeliveryPointNumber.Create(src.SenderPointNumber).Value;
        var senderName = DeliveryPointJurName.Create(src.SenderPointJurName).Value;
        var encloses = src.Encloses?.Select(item => Enclose.Create(item).Value).ToArray();
        var cityTo = CityId.Create(src.CityTo).Value;
        var cityFrom = CityId.Create(src.CityFrom).Value;
        var deliveryMode = DeliveryMode.Create(src.DeliveryMode).Value;
        var receptionDate = ReceptionDate.Create(src.ReceptionDate).Value;
        var linkedInvoice = src.LinkedInvoice is not null 
            ? LinkedInvoice.Create(src.LinkedInvoice.Id, src.LinkedInvoice.PostageType, src.LinkedInvoice.Encloses, src.LinkedInvoice.RecipientPointJurName).Value 
            : null;

        var isUtilization = IsUtilization.Create(src.IsUtilization).Value;
        var isSendToReturn = IsSendToReturn.Create(src.IsSendToReturn).Value;
        var entity = new Invoice(id, postageType, gettingType, recipientPointNumber, recipientPointName, contract, senderNumber, senderName, cityTo, cityFrom, deliveryMode, receptionDate, encloses, linkedInvoice, isUtilization, isSendToReturn);

        return entity;
    }

    private readonly IReadOnlyCollection<PostageType> deliveryPointsPostageTypes = new[] { Models.PostageType.ToDeliveryPoint, Models.PostageType.ToDeliveryPointWithCashOnDelivery };
    private readonly IReadOnlyCollection<GettingType> courierOrWarehouseGettingTypes = new[] { Models.GettingType.CourierCall, Models.GettingType.InWarehouse };
    private readonly IReadOnlyCollection<PostageType> returnsPostageTypes = new[] { Models.PostageType.Return, Models.PostageType.ReturnWithCashOnDelivery };
    private readonly IReadOnlyCollection<PostageType> bc2cReturnPostageTypes = new[] { Models.PostageType.ReturnBC2C, Models.PostageType.ReturnBC2CWithCashOnDelivery };
    private readonly IReadOnlyCollection<PostageType> bc2cPostageTypes = new[] { Models.PostageType.BC2C, Models.PostageType.BC2CCashOnDelivery };
    private readonly IReadOnlyCollection<string> fivePostJurNames = new[] { "5 ПОСТ (ООО)", "ООО \"5 ПОСТ\"", "ООО \"Х5 ОМНИ\"", "ООО \"Х5 ОМНИ\"," };
    private readonly string forbiddenDeliveryPoint = "0001-001";
    private readonly string CompanyBoxIkn = "9990000191";
    private bool IsDeliveryPointPostageType => deliveryPointsPostageTypes.Contains(PostageType);
    private bool IsCourierOrWarehouseGettingType => courierOrWarehouseGettingTypes.Contains(GettingType);
    private bool IsRecipientDeliveryPointHasFivePostJurName => fivePostJurNames.Contains(RecipientPointJurName);
    private bool IsRecipientDeliveryPointNotForbidden => forbiddenDeliveryPoint != RecipientPointNumber;
    private bool IsGettingTypeDeliveryPointByPack => GettingType == Models.GettingType.InDeliveryPointByPack;
    private bool IsIknNotCompanyBox => Ikn != CompanyBoxIkn;
    private bool IsSenderDeliveryPointNotForbidden => forbiddenDeliveryPoint != SenderPointNumber;
    private bool IsReturnPostageType => returnsPostageTypes.Contains(PostageType);
    private bool IsPostReturnPostageType => PostageType.Value == Models.PostageType.PostReturn;
    private bool IsSenderDeliveryPointHasFivePostJurName => fivePostJurNames.Contains(SenderPointJurName);
    private bool IsLinkedInvoicesDeliveryPointHasFivePostJurName => fivePostJurNames.Contains(LinkedInvoice?.RecipientPointJurName);
    private bool IsPostageTypeReturnBc2C => bc2cReturnPostageTypes.Contains(PostageType);
    private bool IsGettingTypeInDeliveryPoint => GettingType == Models.GettingType.InDeliveryPoint;
    private bool IsPostageTypeBc2c => bc2cPostageTypes.Contains(PostageType);
    private bool IsClientReturnPostageType => PostageType.Value == Models.PostageType.ClientReturn;
    
    public TariffZoneType DefineTariffZoneType()
        => this switch {
            {
                IsDeliveryPointPostageType: true, 
                IsCourierOrWarehouseGettingType: true,
                IsRecipientDeliveryPointNotForbidden: true,
                IsRecipientDeliveryPointHasFivePostJurName: false
            } => TariffZoneType.GeneralCourierWarehouseZone,
            
            {
                IsDeliveryPointPostageType: true, 
                IsCourierOrWarehouseGettingType: true,
                IsRecipientDeliveryPointNotForbidden: true,
                IsRecipientDeliveryPointHasFivePostJurName: true
            } => TariffZoneType.GeneralFivePostCourierWarehouseZone,
            
            {
                IsDeliveryPointPostageType: true, 
                IsGettingTypeDeliveryPointByPack: true,
                IsRecipientDeliveryPointNotForbidden: true,
                IsRecipientDeliveryPointHasFivePostJurName: false       
            }  => TariffZoneType.GeneralDeliveryPointZone,
            
            {
                IsDeliveryPointPostageType: true, 
                IsGettingTypeDeliveryPointByPack: true,
                IsRecipientDeliveryPointNotForbidden: true,
                IsRecipientDeliveryPointHasFivePostJurName: true       
            }  => TariffZoneType.GeneralFivePostDeliveryPointZone,
            
            {
                IsDeliveryPointPostageType: true, 
                IsGettingTypeInDeliveryPoint: true
            } => TariffZoneType.None,
            
            {
                IsPostageTypeBc2c: true,
                IsIknNotCompanyBox: true, 
                IsRecipientDeliveryPointNotForbidden: true,
                IsSenderDeliveryPointNotForbidden: true,
                IsRecipientDeliveryPointHasFivePostJurName: false       
            }  => TariffZoneType.BC2CGeneralDeliveryPointZone,
            
            {
                IsPostageTypeBc2c: true,
                IsIknNotCompanyBox: true, 
                IsRecipientDeliveryPointNotForbidden: true,
                IsSenderDeliveryPointNotForbidden: true,
                IsRecipientDeliveryPointHasFivePostJurName: true       
            }  => TariffZoneType.BC2CGeneralFivePostDeliveryPointZone,
            
            {
                IsPostageTypeBc2c: true,
                IsIknNotCompanyBox: false, 
                IsRecipientDeliveryPointNotForbidden: true,
                IsSenderDeliveryPointNotForbidden: true,
            }  => TariffZoneType.C2CZone,
            
            {
                IsReturnPostageType: true, 
                IsLinkedInvoicesDeliveryPointHasFivePostJurName: false
            }  => TariffZoneType.GeneralReturnsZone,
            
            {
                IsPostReturnPostageType: true,
                IsSenderDeliveryPointHasFivePostJurName: false       
            }  => TariffZoneType.GeneralReturnsPostReturnZone,

            {
                IsClientReturnPostageType:true,
                IsSenderDeliveryPointHasFivePostJurName : false
            } => TariffZoneType.GeneralReturnsClientReturnZone,
            
            {
                IsClientReturnPostageType:true,
                IsSenderDeliveryPointHasFivePostJurName : true
            } => TariffZoneType.GeneralFivePostReturnsClientReturnZone,
            
            {
                IsReturnPostageType: true,
                IsLinkedInvoicesDeliveryPointHasFivePostJurName: true       
            }  => TariffZoneType.GeneralFivePostReturnsZone,
            
            {
                IsPostReturnPostageType: true,
                IsSenderDeliveryPointHasFivePostJurName: true       
            }  => TariffZoneType.GeneralFivePostReturnsPostReturnZone,
            
            {
                IsIknNotCompanyBox: true, 
                IsPostageTypeReturnBc2C: true,
                IsRecipientDeliveryPointHasFivePostJurName: false       
            }  => TariffZoneType.BC2CReturnGeneralDeliveryPointZone,
            
            {
                IsIknNotCompanyBox: true, 
                IsPostageTypeReturnBc2C: true,
                IsRecipientDeliveryPointHasFivePostJurName: true       
            }  => TariffZoneType.BC2CReturnGeneralFivePostDeliveryPointZone,
            
            {
                IsIknNotCompanyBox: false, 
                IsPostageTypeReturnBc2C: true,
            }  => TariffZoneType.BC2CReturnGeneralZone,
            
        _ => TariffZoneType.None
    };
    
    public bool AllEnclosesInSameState(IEnumerable<EncloseState> states)
    {
        return states.Any(state => Encloses.All(item => item.State == state));
    }

    public bool AllEnclosesInInitialStates()
    {
        IReadOnlyCollection<EncloseState> initialStates = new[]
        {
            EncloseState.Registered, EncloseState.FormedForPassingToLogistician,
            EncloseState.WaitingForClientDeliveryToPT, EncloseState.ReadyForSending
        };

        return this.Encloses.Any(enclose => initialStates.Contains(enclose.State));
    }
}