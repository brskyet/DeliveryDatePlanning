using System.Linq.Expressions;
using DeliveryDatePlanning.Data.Model.EstimateDb;
using SpeciVacation;

namespace DeliveryDatePlanning.Data.Specification.EstimateDb;

public class HasEncloseKeySpecification : Specification<Estimate>
{
    private readonly string key;
    
    public HasEncloseKeySpecification(string value)
    {
        this.key = value;
    }
    
    public override Expression<Func<Estimate, bool>> ToExpression()
    {
        return estimate => estimate.Invoice.Encloses.Any(item => item.Key == this.key);
    }
}