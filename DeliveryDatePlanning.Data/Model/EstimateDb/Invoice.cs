using AutoMapper;
using DeliveryDatePlanning.Application.Common.Mapping;
using Models;

namespace DeliveryDatePlanning.Data.Model.EstimateDb;

public class Invoice : IMapWith<Domain.Core.Entity.Invoice>
{
    public string Key { get; set; }
    public PostageType PostageType { get; set; }
    public GettingType GettingType { get; set; }
    public string RecipientPointNumber { get; set; }
    public string RecipientPointJurName { get; set; }
    public string Ikn { get; set; }
    public string SenderPointNumber { get; set; }
    public string SenderPointJurName { get; set; }
    public string CityTo { get; set; }
    public string CityFrom { get; set; }
    public int DeliveryMode { get; set; }
    public string ReceptionDate { get; set; }
    public List<Enclose> Encloses { get; set; }
    public LinkedInvoice LinkedInvoice { get; set; }
    public bool IsUtilization { get; set; }
    public bool IsSendToReturn { get; set; }
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Core.Entity.Invoice, Invoice>()
            .ForMember(dst => dst.Key, opt => opt.MapFrom(src => src.Id))
            .ForMember(dst => dst.PostageType, opt => opt.MapFrom(src => src.PostageType.Value))
            .ForMember(dst => dst.GettingType, opt => opt.MapFrom(src => src.GettingType.Value))
            .ForMember(dst => dst.RecipientPointNumber, opt => opt.MapFrom(src => src.RecipientPointNumber.Value))
            .ForMember(dst => dst.RecipientPointJurName, opt => opt.MapFrom(src => src.RecipientPointJurName.Value))
            .ForMember(dst => dst.Ikn, opt => opt.MapFrom(src => src.Ikn.Value))
            .ForMember(dst => dst.SenderPointNumber, opt => opt.MapFrom(src => src.SenderPointNumber.Value))
            .ForMember(dst => dst.SenderPointJurName, opt => opt.MapFrom(src => src.SenderPointJurName.Value))
            .ForMember(dst => dst.CityTo, opt => opt.MapFrom(src => src.CityTo.Value))
            .ForMember(dst => dst.CityFrom, opt => opt.MapFrom(src => src.CityFrom.Value))
            .ForMember(dst => dst.DeliveryMode, opt => opt.MapFrom(src => src.DeliveryMode.Value))
            .ForMember(dst => dst.ReceptionDate, opt => opt.MapFrom(src => src.ReceptionDate.Value.ToString("O")))
            ;
    }
}