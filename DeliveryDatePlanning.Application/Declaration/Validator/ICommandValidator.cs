using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Domain.Core.Entity;

namespace DeliveryDatePlanning.Application.Declaration.Validator;

public interface ICommandValidator
{
    bool IsValid(EstimateCommand command, Invoice invoice, Estimate estimate);
    
    bool IsValid(EncloseStateRecordCommand command, Estimate estimate);

    bool IsValid(InvoiceDataChangedCommand command, Invoice invoice, Estimate estimate);
}