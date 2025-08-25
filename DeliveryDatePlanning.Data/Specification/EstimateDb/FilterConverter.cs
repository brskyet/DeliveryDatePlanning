using System.Linq.Expressions;
using DeliveryDatePlanning.Application.Declaration.Store;
using DeliveryDatePlanning.Data.Model.EstimateDb;
using SpeciVacation;

namespace DeliveryDatePlanning.Data.Specification.EstimateDb;

public static class FilterConverter
{
    public static Expression<Func<Estimate, bool>> ToExpression(this EstimateFilter filter)
    {
        var spec = Specification<Estimate>.All;

        if (filter.InvoiceKey is not null)
        {
            spec = spec.And(new HasInvoiceKeySpecification(filter.InvoiceKey));
        }
        
        if (filter.EncloseKey is not null)
        {
            spec = spec.And(new HasEncloseKeySpecification(filter.EncloseKey));
        }

        return spec.ToExpression();
    }
}