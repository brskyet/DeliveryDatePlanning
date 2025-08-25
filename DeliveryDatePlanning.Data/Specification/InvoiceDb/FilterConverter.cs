using System.Linq.Expressions;
using DeliveryDatePlanning.Application.Declaration.Store;
using DeliveryDatePlanning.Data.Model.Apt;
using SpeciVacation;

namespace DeliveryDatePlanning.Data.Specification.InvoiceDb;

public static class FilterConverter
{
    public static Expression<Func<Invoice, bool>> ToExpression(this InvoiceFilter filter)
    {
        if (filter.InvoiceKey is not null)
        {
            return Specification<Invoice>.All.And(new HasInvoiceKeySpecification(filter.InvoiceKey)).ToExpression();
        }
        
        if (filter.EncloseKey is not null)
        {
            return Specification<Invoice>.All.And(new HasEncloseKeySpecification(filter.EncloseKey)).ToExpression();
        }

        return Specification<Invoice>.All.ToExpression();
    }
}