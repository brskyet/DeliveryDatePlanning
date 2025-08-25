using DeliveryDatePlanning.Domain.Core.Entity;

namespace DeliveryDatePlanning.Application.Declaration.Store;

public interface ITariffZoneKeyStore
{
    TariffZoneKey[] All();
}