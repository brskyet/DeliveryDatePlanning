using System.Linq.Expressions;
using DeliveryDatePlanning.Data.Model.Apt;
using Models;
using SpeciVacation;

namespace DeliveryDatePlanning.Data.Specification.InvoiceDb;

public class HasInvoiceKeySpecification : Specification<Invoice>
{
    private readonly CompanyKey key;
    
    public HasInvoiceKeySpecification(string value)
    {
        this.key = CompanyKey.Parse(value);
    }
    
    public override Expression<Func<Invoice, bool>> ToExpression()
    {
        return invoice => invoice.Id == this.key.Id && invoice.OwnerId == this.key.OwnerId;
    }
}