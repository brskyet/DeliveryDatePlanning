using DeliveryDatePlanning.Application.Common.Constant;
using DeliveryDatePlanning.Application.Common.Exception;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;
using DeliveryDatePlanning.Application.Declaration.Validator;
using DeliveryDatePlanning.Domain.Core.Entity;
using Models;

namespace DeliveryDatePlanning.Application.Core.Validator;

public class CommandValidator : ICommandValidator
{
    private readonly IReadOnlyCollection<PostageType> postageTypes = new[]
    {
        PostageType.ToDeliveryPoint,
        PostageType.ToDeliveryPointWithCashOnDelivery,
        PostageType.Return,
        PostageType.ReturnWithCashOnDelivery,
        PostageType.ClientReturn,
        PostageType.PostReturn,
        PostageType.BC2C,
        PostageType.BC2CCashOnDelivery,
        PostageType.ReturnBC2C,
        PostageType.ReturnBC2CWithCashOnDelivery,
    };

    public const string ForbiddenDeliveryPoint = "0001-001";

    public bool IsValid(EstimateCommand command, Invoice invoice, Estimate estimate)
    {
        if (postageTypes.Contains(invoice.PostageType) == false)
        {
            return false;
        }

        switch (command.Exchange)
        {
            case ExchangeName.EncloseRegistered when estimate is not null:
            case ExchangeName.EncloseRegistered when this.isInvoiceHasReturnPostageType(invoice):
            case ExchangeName.InvoiceRedirected when this.isInvoiceRedirectedValid(estimate) == false:
            case ExchangeName.InvoiceDeliveryModeChanged when estimate is null: // for cases when 1143 received is about 200 ms early than 101
            case ExchangeName.InvoiceDeliveryModeChanged when invoice.AllEnclosesInInitialStates() == false:
                return false;
            default:
                return true;
        }
    }

    public bool IsValid(EncloseStateRecordCommand command, Estimate estimate)
    {
        if (estimate is null)
        {
            return false; // estimate entity must be exist
        }
        
        if (estimate.Invoice is null)
        {
            throw new InternalException($"[{nameof(CommandValidator)}] Estimate must have Invoice field");
        }

        if (estimate.Invoice.Encloses.All(i => i.Id != command.Key))
        {
            return false;
        }

        if (command.Timestamp == default)
        {
            return false;
        }
        
        return true;
    }

    public bool IsValid(InvoiceDataChangedCommand command, Invoice invoice, Estimate estimate)
    {
        if (invoice is null)
        {
            throw new InternalException($"[{nameof(InvoiceDataChangedCommand)}] Invoice entity must be exist");
        }
        
        if (estimate is null)
        {
            throw new InternalException($"[{nameof(InvoiceDataChangedCommand)}] Estimate entity must be exist");
        }
        
        switch (command.Exchange)
        {
            case ExchangeName.InvoiceRedirected when this.isInvoiceRedirectedValid(estimate) == false:
                return true;
            default:
                return false;
        }
    }

    private bool isInvoiceRedirectedValid(Estimate estimate)
    {
        if (estimate is null)
        {
            throw new InternalException($"[{nameof(CommandValidator)}] Estimate entity must be exist");
        }

        if (estimate.Invoice.RecipientPointNumber.Value != ForbiddenDeliveryPoint && estimate.IsDeliveryDateEmpty())
        {
            return false;
        }
        
        if (estimate.Invoice.RecipientPointNumber.Value == ForbiddenDeliveryPoint && estimate.IsDeliveryDateEmpty() == false)
        {
            return false;
        }
        

        return true;
    }

    private readonly IReadOnlyCollection<PostageType> returnPostageTypes = new[]
    {
        PostageType.Return,  PostageType.ReturnWithCashOnDelivery, PostageType.ClientReturn,
        PostageType.ReturnBC2C, PostageType.ReturnBC2CWithCashOnDelivery
    };

    private bool isInvoiceHasReturnPostageType(Invoice invoice) => this.returnPostageTypes.Contains(invoice.PostageType.Value);

}