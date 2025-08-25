using System.Text.RegularExpressions;
using DeliveryDatePlanning.Application.Declaration.Handler.Query;
using DeliveryDatePlanning.Application.Declaration.Validator;

namespace DeliveryDatePlanning.Application.Core.Validator;

public class QueryValidator : IQueryValidator
{
    public bool IsValid(EstimateReadQuery query)
    {
        if (string.IsNullOrEmpty(query.InvoiceKey) || Regex.IsMatch(query.InvoiceKey, @"(^[0-9]+)[-]([0-9]+$)") == false)
        {
            return false;
        }

        return true;
    }
}