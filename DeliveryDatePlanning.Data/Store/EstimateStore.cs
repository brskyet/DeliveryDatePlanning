using System.Globalization;
using AutoMapper;
using CSharpFunctionalExtensions;
using MongoDB.Bson;
using MongoDB.Driver;
using DeliveryDatePlanning.Application.Common.Exception;
using DeliveryDatePlanning.Application.Declaration.Store;
using DeliveryDatePlanning.Data.Context;
using DeliveryDatePlanning.Data.Filter;
using DeliveryDatePlanning.Data.Model.EstimateDb;
using DeliveryDatePlanning.Data.Specification.EstimateDb;
using DeliveryDatePlanning.Domain.Core.ValueObject.Enclose;
using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;
using DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;
using DeliveryDatePlanning.Domain.Enums;
using Models;

namespace DeliveryDatePlanning.Data.Store;

public class EstimateStore : IEstimateStore
{
    private readonly EstimateDbContext context;
    private readonly IMapper mapper;
    private readonly IReadOnlyCollection<EncloseState> states = new[]
    {
        EncloseState.DeliveredToPT, EncloseState.Received, EncloseState.ReturnedToClient, EncloseState.Rejected, EncloseState.Annulled
    };

    public EstimateStore(EstimateDbContext context, IMapper mapper)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
    }

    public async Task<Domain.Core.Entity.Estimate> FindOrDefault(EstimateFilter filter, CancellationToken token = default)
    {
        var query = this.context.EstimateCollection.Aggregate().Match(filter.ToExpression());

        var document = await query.FirstOrDefaultAsync(token);

        if (document is not null)
        {
            var entity = this.estimateFactory(document);
            
            if (entity.IsFailure)
            {
                throw new InternalException($"[{nameof(EstimateStore)}] Estimate entity is not valid");
            }
            
            return entity.Value;
        }

        return default;
    }

    public async Task Upsert(Domain.Core.Entity.Estimate entity, CancellationToken token = default)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }

        var document = this.mapper.Map<Estimate>(entity);

        await this.context.EstimateCollection.FindOneAndReplaceAsync<Estimate>(
            item => item.Id == document.Id, 
            document, 
            new FindOneAndReplaceOptions<Estimate> { IsUpsert = true }, 
            token);
    }

    public async Task UpdateEncloseStateRecord(Domain.Core.Entity.Estimate entity, CancellationToken token = default)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        
        var document = this.mapper.Map<Estimate>(entity);

        var filterDefinition = Builders<Estimate>.Filter.Eq(x => x.Id, document.Id);
        
        var updateDefinition = Builders<Estimate>.Update
            .Set(x => x.Invoice.Encloses, document.Invoice.Encloses);
        
        await this.context.EstimateCollection.UpdateOneAsync(filterDefinition, updateDefinition, cancellationToken: token);
    }    
    
    public async Task UpdateReceptionDate(Domain.Core.Entity.Estimate entity, CancellationToken token = default)
    {
        if (entity is null)
        {
            throw new ArgumentNullException(nameof(entity));
        }
        
        var document = this.mapper.Map<Estimate>(entity);

        var filterDefinition = Builders<Estimate>.Filter.Eq(x => x.Id, document.Id);
        
        var updateDefinition = Builders<Estimate>.Update
            .Set(x => x.Invoice.ReceptionDate, document.Invoice.ReceptionDate);
        
        await this.context.EstimateCollection.UpdateOneAsync(filterDefinition, updateDefinition, cancellationToken: token);
    }

    public async Task<string[]> RelevantDayChangedForStatus(string cityTo, int limit = default, CancellationToken token = default)
    {
        var filter = new DayChangedForStatusFilterFactory().Create(cityTo);

        var result = await this.findRelevant(filter, limit, token);

        return result;
    }

    public async Task<string[]> RelevantDayChangedForOverdue(string cityTo, int limit = default, CancellationToken token = default)
    {
        var pipeline = new BsonDocument[]
        {
            new("$match",
                new BsonDocument
                {
                    { "Status", EstimateStatusType.Expired },
                    { "Invoice.CityTo", cityTo },
                    {
                        "DateStart",
                        new BsonDocument("$ne", BsonNull.Value)
                    },
                    {
                        "DateEnd",
                        new BsonDocument("$ne", BsonNull.Value)
                    },
                    {
                        "Invoice.Encloses.State",
                        new BsonDocument("$nin",
                            new BsonArray(states))
                    }
                }),
            new("$unwind",
                new BsonDocument("path", "$Invoice.Encloses")),
            new("$unwind",
                new BsonDocument("path", "$Invoice.Encloses.StateHistory")),
            new("$group",
                new BsonDocument
                {
                    { "_id", "$Invoice.Key" },
                    {
                        "states",
                        new BsonDocument("$addToSet", "$Invoice.Encloses.StateHistory.State")
                    }
                }),
            new("$match",
                new BsonDocument("states",
                    new BsonDocument("$nin",
                        new BsonArray(states)))),
            new("$project",
                new BsonDocument("_id", 1))
        };

        var pResult = await this.context.EstimateCollection.Aggregate<BsonDocument>(pipeline).ToListAsync(token);

        var result = pResult.Select(x => x["_id"].AsString).Distinct().ToArray();

        return result;
    }

    public async Task<string[]> RelevantDayChangedForNotAccepted(string cityTo, int limit = default, CancellationToken token = default)
    {
        var filter = new DayChangedForNotAcceptedFilterFactory().Create(cityTo);

        var result = await this.findRelevant(filter, limit, token);

        return result;
    }

    public async Task UpdateInvoice(EstimateFilter filter, Domain.Core.Entity.Invoice invoice, CancellationToken token = default)
    {
        var invoiceMongo = this.mapper.Map<Invoice>(invoice);
        
        var filterDefinition = Builders<Estimate>.Filter.Eq(x => x.Invoice.Key, filter.InvoiceKey);
        var updateDefinition = Builders<Estimate>.Update
            .Set(x => x.Invoice, invoiceMongo);
        
        await this.context.EstimateCollection.UpdateOneAsync(filterDefinition, updateDefinition, cancellationToken: token);
    }

    private async Task<string[]> findRelevant(FilterDefinition<Estimate> filter, int limit = default, CancellationToken token = default)
    {
        var projection = Builders<Estimate>.Projection.Expression(document => document.Invoice.Key);

        var query = this.context.EstimateCollection.Aggregate().Match(filter).Project(projection);

        if (limit > 0)
        {
            query = query.Limit(limit);
        }
        
        var result = await query.ToListAsync(token);

        return result.ToArray();
    }

    private Result<Domain.Core.Entity.Estimate> estimateFactory(Estimate document)
    {
        var dateStart = this.parseDateOnly(document.DateStart);
        var dateEnd = this.parseDateOnly(document.DateEnd);
        var deliveryDates = DeliveryDates.Create(dateStart, dateEnd).Value;
        var status = EstimateStatusTypeEnum.Create(document.Status).Value;
        var reason = DateChangeReasonTypeEnum.Create(document.Reason).Value;
        var overdue = OverdueDays.Create(document.Overdue).Value;
        var invoice = this.invoiceFactory(document.Invoice).Value;

        var result = Domain.Core.Entity.Estimate.Create(document.Id, deliveryDates, status, reason, overdue, invoice);

        return result;
    }
    
    private Result<Domain.Core.Entity.Invoice> invoiceFactory(Invoice document)
    {
        var postageType = PostageTypeEnum.Create(document.PostageType).Value;
        var gettingType = GettingTypeEnum.Create(document.GettingType).Value;
        var recipientNumber = DeliveryPointNumber.Create(document.RecipientPointNumber).Value;
        var recipientName = DeliveryPointJurName.Create(document.RecipientPointJurName).Value;
        var contract = ContractNumber.Create(document.Ikn).Value;
        var senderNumber = DeliveryPointNumber.Create(document.SenderPointNumber).Value;
        var senderName = DeliveryPointJurName.Create(document.SenderPointJurName).Value;
        var cityToId = CityId.Create(document.CityTo).Value;
        var cityFromId = CityId.Create(document.CityFrom).Value;
        var priority = DeliveryMode.Create(document.DeliveryMode).Value;
        var receptionDate = ReceptionDate.Create(document.ReceptionDate).Value;
        var encloses = document.Encloses.Select(item => this.encloseFactory(item).Value).ToArray();
        var linkedInvoice = document.LinkedInvoice is not null ? this.linkedInvoiceFactory(document.LinkedInvoice).Value : null;

        var isUtilization = IsUtilization.Create(document.IsUtilization).Value;
        var isSendToReturn = IsSendToReturn.Create(document.IsSendToReturn).Value;

        var result = Domain.Core.Entity.Invoice.Create(document.Key, postageType, gettingType, recipientNumber, recipientName, contract, senderNumber, senderName, cityToId, cityFromId, priority, receptionDate, encloses, linkedInvoice, isUtilization, isSendToReturn);

        return result;
    }

    private Result<Domain.Core.Entity.Enclose> encloseFactory(Enclose document)
    {
        var state = EncloseStateEnum.Create(document.State).Value;
        var encloseStates = document.StateHistory.Select(item => 
            Domain.Core.ValueObject.Enclose.EncloseStateHistoryRecord.Create(item.Timestamp, item.State).Value).ToArray();
        
        return Domain.Core.Entity.Enclose.Create(document.Key, state, encloseStates);
    }

    private Result<Domain.Core.Entity.LinkedInvoice> linkedInvoiceFactory(LinkedInvoice document)
    {
        var postageType = PostageTypeEnum.Create(document.PostageType).Value;
        var encloses = document.Encloses.Select(item => this.encloseFactory(item).Value).ToArray();
        var recipientPointJurName = DeliveryPointJurName.Create(document.RecipientPointJurName).Value;
        return Domain.Core.Entity.LinkedInvoice.Create(document.Key, postageType, encloses, recipientPointJurName);
    }
    
    private DateOnly? parseDateOnly(string value)
    {
        return DateOnly.TryParseExact(value, new []{"dd.MM.yyyy"}, CultureInfo.InvariantCulture, DateTimeStyles.None, out var result) 
            ? result
            : null;
    }
}