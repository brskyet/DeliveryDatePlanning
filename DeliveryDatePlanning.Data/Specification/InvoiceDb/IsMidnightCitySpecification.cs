using System.Linq.Expressions;
using DeliveryDatePlanning.Data.Model.Apt;
using SpeciVacation;

namespace DeliveryDatePlanning.Data.Specification.InvoiceDb;

public class IsMidnightCitySpecification: Specification<City>
{
    private readonly int hour;
    
    public IsMidnightCitySpecification(int hour)
    {
        this.hour = hour;
    }
    
    public override Expression<Func<City, bool>> ToExpression()
    {
        return c => c.Region.TimeZone + this.hour == 0 || c.Region.TimeZone + this.hour == 24;
    }
}
