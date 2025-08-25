using AutoMapper;
using DeliveryDatePlanning.Application.Common.Mapping;
using DeliveryDatePlanning.Domain.Enums;

namespace DeliveryDatePlanning.Application.Declaration.Handler.Query;

public class EstimateVm : IMapWith<Domain.Core.Entity.Estimate>
{
    public string DateStart { get; private set; }

    public string DateEnd { get; private set;}

    public EstimateStatusType Status { get; private set; }

    public DateChangeReasonType Reason { get; private set; }

    public int? Overdue { get; private set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Core.Entity.Estimate, EstimateVm>()
            .ForMember(dst => dst.DateStart, opt =>
                opt.MapFrom(src =>
                    src.DeliveryDates.DateStart.HasValue ? src.DeliveryDates.DateStart.Value.ToString("dd.MM.yyyy") : null))
            .ForMember(dst => dst.DateEnd, opt =>
                opt.MapFrom(src =>
                    src.DeliveryDates.DateEnd.HasValue ? src.DeliveryDates.DateEnd.Value.ToString("dd.MM.yyyy") : null))
            .ForMember(dst => dst.Status, opt => opt.MapFrom(src => src.Status.Value))
            .ForMember(dst => dst.Reason, opt => opt.MapFrom(src => src.Reason.Value))
            .ForMember(dst => dst.Overdue, opt => opt.MapFrom(src => src.Overdue.Value));
    }
}