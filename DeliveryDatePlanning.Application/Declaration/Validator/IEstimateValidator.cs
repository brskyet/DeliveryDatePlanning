using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Domain.Core.Entity;

namespace DeliveryDatePlanning.Application.Declaration.Validator;

public interface IEstimateValidator
{
    bool IsValid(Estimate before, Result<Estimate> after);
}