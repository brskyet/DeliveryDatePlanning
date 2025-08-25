using AutoMapper;
using DeliveryDatePlanning.Application.Common.Mapping;
using DeliveryDatePlanning.Domain.Enums;

namespace DeliveryDatePlanning.Data.Model.EstimateDb;

public class Estimate : IMapWith<Domain.Core.Entity.Estimate>
{
    public string Id { get; set; }
    
    public string DateStart { get; set; }
    
    public string DateEnd { get; set; }

    public EstimateStatusType Status { get; set; }

    public DateChangeReasonType Reason { get; set; }
    
    public int? Overdue { get; set; }

    public Invoice Invoice { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Core.Entity.Estimate, Estimate>()
            .ForMember(dst => dst.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dst => dst.DateStart, opt =>
                opt.MapFrom(src => src.DeliveryDates.DateStart.HasValue ? src.DeliveryDates.DateStart.Value.ToString("dd.MM.yyyy")  : null))
            .ForMember(dst => dst.DateEnd, opt => 
                opt.MapFrom(src => src.DeliveryDates.DateEnd.HasValue ? src.DeliveryDates.DateEnd.Value.ToString("dd.MM.yyyy")  : null))
            .ForMember(dst => dst.Status, opt => opt.MapFrom(src => src.Status.Value))
            .ForMember(dst => dst.Reason, opt => opt.MapFrom(src => src.Reason.Value))
            .ForMember(dst => dst.Overdue, opt => opt.MapFrom(src => src.Overdue.Value))
            ;
    }
}