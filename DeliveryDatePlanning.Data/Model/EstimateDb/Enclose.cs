using AutoMapper;
using DeliveryDatePlanning.Application.Common.Mapping;
using Models;

namespace DeliveryDatePlanning.Data.Model.EstimateDb;

public class Enclose : IMapWith<Domain.Core.Entity.Enclose>
{
    public string Key { get; set; }
    public EncloseState State { get; set; }
    public List<EncloseStateHistoryRecord> StateHistory { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Core.Entity.Enclose, Enclose>()
            .ForMember(dst => dst.Key, opt => opt.MapFrom(src => src.Id))
            .ForMember(dst => dst.State, opt => opt.MapFrom(src => src.State.Value))
            ;
    }
}