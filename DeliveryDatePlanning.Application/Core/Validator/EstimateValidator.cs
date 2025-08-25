using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Application.Common.Exception;
using DeliveryDatePlanning.Application.Declaration.Validator;
using DeliveryDatePlanning.Domain.Core.Entity;

namespace DeliveryDatePlanning.Application.Core.Validator;

public class EstimateValidator : IEstimateValidator
{
    public bool IsValid(Estimate before, Result<Estimate> after)
    {
        if (after.IsFailure)
        {
            throw new InternalException(after.Error);
        }
        
        if (after.Value == before)
        {
            return false;
        }
        
        return true;
    }
}