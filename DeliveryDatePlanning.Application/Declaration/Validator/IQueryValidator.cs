using DeliveryDatePlanning.Application.Declaration.Handler.Query;

namespace DeliveryDatePlanning.Application.Declaration.Validator;

public interface IQueryValidator
{
    bool IsValid(EstimateReadQuery query);
}