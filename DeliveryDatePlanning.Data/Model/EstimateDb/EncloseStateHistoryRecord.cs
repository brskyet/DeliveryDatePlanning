using AutoMapper;
using DeliveryDatePlanning.Application.Common.Mapping;
using Models;

namespace DeliveryDatePlanning.Data.Model.EstimateDb;

public class EncloseStateHistoryRecord : IMapWith<Domain.Core.ValueObject.Enclose.EncloseStateHistoryRecord>
{
    public string Timestamp { get; set; }
    public EncloseState State { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<Domain.Core.ValueObject.Enclose.EncloseStateHistoryRecord, EncloseStateHistoryRecord>()
            .ForMember(dst => dst.Timestamp, opt => opt.MapFrom(src => src.Timestamp.ToString("O")))
            .ForMember(dst => dst.State, opt => opt.MapFrom(src => src.State))
            ;
    }
}