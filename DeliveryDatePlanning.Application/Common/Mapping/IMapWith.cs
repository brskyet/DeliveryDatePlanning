using AutoMapper;

namespace DeliveryDatePlanning.Application.Common.Mapping;

public interface IMapWith<T>
{
    void Mapping(Profile profile) => profile.CreateMap(typeof(T), GetType());
}