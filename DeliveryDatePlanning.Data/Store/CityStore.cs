using Microsoft.EntityFrameworkCore;
using DeliveryDatePlanning.Application.Declaration.Store;
using DeliveryDatePlanning.Data.Context;
using DeliveryDatePlanning.Data.Specification.InvoiceDb;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;

namespace DeliveryDatePlanning.Data.Store;

public class CityStore : ICityStore
{
    private AptContext Context { get; }
    
    public CityStore(AptContext context)
    {
        this.Context = context;
    }

    public async Task<City[]> FindMidnightCities(DateTime timestamp, CancellationToken token)
    {
        var spec = new IsMidnightCitySpecification(timestamp.Hour);
        
        var cities = await this.Context.Cities
            .Where(spec.ToExpression())
            .Select(c => new
            {
                c.Key,
                c.Region.TimeZone
            })
            .ToListAsync(cancellationToken: token);

        return cities.Select(c => City.Create(c.Key.ToString(), CityTimeZone.Create(c.TimeZone).Value).Value).ToArray();
    }
}