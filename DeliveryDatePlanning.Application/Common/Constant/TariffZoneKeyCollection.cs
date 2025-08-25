using System.Collections;
using DeliveryDatePlanning.Application.Declaration.Store;
using DeliveryDatePlanning.Domain.Core.Entity;

namespace DeliveryDatePlanning.Application.Common.Constant;

public class TariffZoneKeyCollection : IReadOnlyCollection<TariffZoneKey>
{
    private readonly IReadOnlyCollection<TariffZoneKey> collection;

    public TariffZoneKeyCollection(ITariffZoneKeyStore store)
    {
        this.collection = store.All();
    }
    
    public IEnumerator<TariffZoneKey> GetEnumerator()
    {
        return this.collection.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }

    public int Count => this.collection.Count;
}