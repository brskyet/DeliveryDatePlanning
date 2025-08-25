using DeliveryDatePlanning.Domain.Core.Entity;

namespace DeliveryDatePlanning.Application.Declaration.Store;

public interface ICityStore
{
    Task<City[]> FindMidnightCities(DateTime timestamp, CancellationToken token);
}