using System.Linq.Expressions;
using DeliveryDatePlanning.Data.Model.Apt;
using Models;
using SpeciVacation;

namespace DeliveryDatePlanning.Data.Specification.InvoiceDb;

public class HasEncloseKeySpecification : Specification<Invoice>
{
    private readonly CompanyKey key;
    
    public HasEncloseKeySpecification(string value)
    {
        this.key = CompanyKey.Parse(value);
    }
    
    public override Expression<Func<Invoice, bool>> ToExpression()
    {
        return invoice => invoice.Encloses.Any(item => item.Id == this.key.Id && item.OwnerId == this.key.OwnerId);
    }
}