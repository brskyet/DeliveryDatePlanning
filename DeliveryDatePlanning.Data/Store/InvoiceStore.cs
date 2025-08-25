using CSharpFunctionalExtensions;
using Microsoft.EntityFrameworkCore;
using DeliveryDatePlanning.Application.Common.Exception;
using DeliveryDatePlanning.Application.Declaration.Store;
using DeliveryDatePlanning.Data.Context;
using DeliveryDatePlanning.Data.Specification.InvoiceDb;
using DeliveryDatePlanning.Domain.Core.Entity;
using DeliveryDatePlanning.Domain.Core.ValueObject.Enclose;
using DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;
using Models;
using Enclose = DeliveryDatePlanning.Data.Model.Apt.Enclose;
using Invoice = DeliveryDatePlanning.Data.Model.Apt.Invoice;

namespace DeliveryDatePlanning.Data.Store;

public class InvoiceStore : IInvoiceStore
{
    private readonly AptContext context;

    public InvoiceStore(AptContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<Domain.Core.Entity.Invoice> Find(InvoiceFilter filter, CancellationToken token = default)
    {
        var dbEntity = await this.context.Invoices
            .Include(i => i.Encloses)
                .ThenInclude(e => e.EncloseStates)
            .Include(i => i.Contract)
            .Include(i => i.InvoiceDeliveryPoint)
                .ThenInclude(idp => idp.DeliveryPoint)
                    .ThenInclude(dp => dp.ClientDeliveryPoint)
                        .ThenInclude(cdp => cdp.Client)
            .Include(i => i.InvoiceDeliveryPoint)
                .ThenInclude(idp => idp.SourceDeliveryPoint)
                    .ThenInclude(sdp => sdp.ClientDeliveryPoint)
                        .ThenInclude(cdp => cdp.Client)
            .Include(i => i.DestinationAddress)
            .Include(i => i.SourceAddress)
            .Include(i=> i.InvoiceAttribute)
            .Include(i => i.DocumentTitle)
                .ThenInclude(dt => dt.DocumentBody)
                     .ThenInclude(db => db.Invoice)
                        .ThenInclude(linkedInvoice => linkedInvoice.Encloses)   
                            .ThenInclude(e => e.EncloseStates)
            .Include(i => i.DocumentTitle)
                .ThenInclude(dt => dt.DocumentBody)
                    .ThenInclude(db => db.Invoice)
                        .ThenInclude(i => i.InvoiceDeliveryPoint)
                            .ThenInclude(idp => idp.DeliveryPoint)
                                .ThenInclude(dp => dp.ClientDeliveryPoint)
                                    .ThenInclude(c => c.Client)
            .Include(i => i.IntParams)
            .FirstOrDefaultAsync(filter.ToExpression(), token);

        if (dbEntity is null)
        {
            throw new NotFoundException($"[{nameof(InvoiceStore)}] Invoice not found. Filter: {filter}");
        }

        var entity = this.invoiceFactory(dbEntity);

        if (entity.IsFailure)
        {
            throw new InternalException($"[{nameof(InvoiceStore)}] Invoice entity is not valid");
        }

        return entity.Value;
    }
    
    public async Task<Domain.Core.Entity.Invoice> FindLinked(InvoiceFilter filter, CancellationToken token = default)
    {
        var dbEntity = await this.context.Invoices
            .Include(i => i.DocumentTitle)
                 .ThenInclude(dt => dt.DocumentBody)
                    .ThenInclude(db => db.Invoice)
            .FirstOrDefaultAsync(filter.ToExpression(), token);
        
        if (dbEntity is null || dbEntity.DocumentTitle?.DocumentBody?.Invoice is null)
        {
            throw new NotFoundException($"[{nameof(InvoiceStore)}] Linked Invoice not found. Filter: {filter}");
        }

        var linkedInvoiceKey = new CompanyKey(dbEntity.DocumentTitle.DocumentBody.Invoice.Id, dbEntity.DocumentTitle.DocumentBody.Invoice.OwnerId).ToString();
        filter = new InvoiceFilter(linkedInvoiceKey, null);
        
        return await this.Find(filter, token);
    }
    
    private Result<Domain.Core.Entity.Invoice> invoiceFactory(Invoice dbEntity)
    {
        var postageType = PostageTypeEnum.Create(dbEntity.PostageType).Value;
        var gettingType = GettingTypeEnum.Create(dbEntity.GettingType).Value;
        var recipientNumber = DeliveryPointNumber.Create(dbEntity.InvoiceDeliveryPoint?.DeliveryPoint?.Number).Value;
        var recipientName = DeliveryPointJurName.Create(dbEntity.InvoiceDeliveryPoint?.DeliveryPoint?.ClientDeliveryPoint?.Client?.CompanyName).Value;
        var contract = ContractNumber.Create(dbEntity.Contract?.NumberContract).Value;
        var senderNumber = DeliveryPointNumber.Create(dbEntity.InvoiceDeliveryPoint?.SourceDeliveryPoint?.Number).Value;
        var senderName = DeliveryPointJurName.Create(dbEntity.InvoiceDeliveryPoint?.SourceDeliveryPoint?.ClientDeliveryPoint?.Client?.CompanyName).Value;
        var cityToId = CityId.Create($"{dbEntity.DestinationAddress?.CityId}-{dbEntity.DestinationAddress?.CityOwnerId}").Value;
        var cityFromId = CityId.Create($"{dbEntity.SourceAddress?.CityId}-{dbEntity.SourceAddress?.CityOwnerId}").Value;
        var priority = DeliveryMode.Create(dbEntity.InvoiceAttribute?.DeliveryMode).Value;
        var receptionDate = ReceptionDate.Create(dbEntity.ReceptionDate).Value;

        var encloses = dbEntity.Encloses.Select(item => this.encloseFactory(item).Value).ToArray();
        var linkedInvoice =  dbEntity.DocumentTitle?.DocumentBody?.Invoice != null 
                    ? this.linkedInvoiceFactory(dbEntity.DocumentTitle.DocumentBody.Invoice).Value
                    : null;

        var isUtiization = IsUtilization.Create(dbEntity.IntParams.FirstOrDefault(p => p.Id == 169 && p.Value == 1) is not null).Value;
        var isSendToReturn = IsSendToReturn.Create(dbEntity.IntParams.FirstOrDefault(p => p.Id == 147 && p.Value == 1) is not null).Value;

        var result = Domain.Core.Entity.Invoice.Create(new CompanyKey(dbEntity.Id, dbEntity.OwnerId).ToString(), postageType, gettingType, recipientNumber, recipientName, contract, senderNumber, senderName, cityToId, cityFromId, priority, receptionDate, encloses, linkedInvoice, isUtiization, isSendToReturn);

        return result;
    }

    private Result<Domain.Core.Entity.Enclose> encloseFactory(Enclose dbEntity)
    {
        var id = new CompanyKey(dbEntity.Id, dbEntity.OwnerId).ToString();
        var state = EncloseStateEnum.Create(dbEntity.EncloseState).Value;
        var encloseStates = dbEntity.EncloseStates?.Select(item => 
            EncloseStateHistoryRecord.Create(item.ChangeDateTime, item.State).Value).ToArray();
        
        return Domain.Core.Entity.Enclose.Create(id, state, encloseStates);
    }

    private Result<LinkedInvoice> linkedInvoiceFactory(Invoice dbEntity)
    {
        var id = new CompanyKey(dbEntity.Id, dbEntity.OwnerId).ToString();
        var postageType = PostageTypeEnum.Create(dbEntity.PostageType).Value;
        var encloses = dbEntity.Encloses?.Select(item => this.encloseFactory(item).Value).ToArray();
        var recipientPointJurName = DeliveryPointJurName.Create(dbEntity.InvoiceDeliveryPoint?.DeliveryPoint?.ClientDeliveryPoint?.Client?.CompanyName).Value; 
        
        return LinkedInvoice.Create(id, postageType, encloses, recipientPointJurName);
    }
}