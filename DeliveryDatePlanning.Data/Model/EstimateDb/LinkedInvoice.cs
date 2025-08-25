using AutoMapper;
using DeliveryDatePlanning.Application.Common.Mapping;
using Models;

namespace DeliveryDatePlanning.Data.Model.EstimateDb;
/// <summary>
/// Связанное отправление
/// </summary>
public class LinkedInvoice : IMapWith<Domain.Core.Entity.LinkedInvoice>
{
    public string Key { get; set; }
    public PostageType PostageType { get; set; }
    public List<Enclose> Encloses { get; set; }
    public string RecipientPointJurName { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Core.Entity.LinkedInvoice, LinkedInvoice>()
            .ForMember(dst => dst.Key, opt => opt.MapFrom(src => src.Id))
            .ForMember(dst => dst.PostageType, opt => opt.MapFrom(src => src.PostageType.Value))
            .ForMember(dst => dst.RecipientPointJurName, opt => opt.MapFrom(src => src.RecipientPointJurName.Value))
            ;
    }
}