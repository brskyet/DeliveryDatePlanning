using System.Linq.Expressions;
using DeliveryDatePlanning.Data.Model.EstimateDb;
using SpeciVacation;

namespace DeliveryDatePlanning.Data.Specification.EstimateDb;

public class HasInvoiceKeySpecification : Specification<Estimate>
{
    private readonly string key;
    
    public HasInvoiceKeySpecification(string value)
    {
        this.key = value;
    }
    
    public override Expression<Func<Estimate, bool>> ToExpression()
    {
        return estimate => estimate.Invoice.Key == this.key;
    }
}