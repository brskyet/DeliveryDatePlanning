using System.Text.Json;
using AutoMapper;
using Microsoft.Extensions.Logging;
using DeliveryDatePlanning.Application.Common.Exception;
using DeliveryDatePlanning.Application.Declaration.Handler;
using DeliveryDatePlanning.Application.Declaration.Handler.Query;
using DeliveryDatePlanning.Application.Declaration.Store;
using DeliveryDatePlanning.Application.Declaration.Validator;

namespace DeliveryDatePlanning.Application.Core.Handler;

public class QueryHandler : IQueryHandler
{
    private readonly IQueryValidator queryValidator;
    private readonly IEstimateStore estimateStore;
    private readonly IMapper mapper;
    private readonly ILogger<QueryHandler> logger;

    public QueryHandler(IQueryValidator queryValidator, IEstimateStore estimateStore, IMapper mapper, ILogger<QueryHandler> logger)
    {
        this.queryValidator = queryValidator ?? throw new ArgumentNullException(nameof(queryValidator));
        this.estimateStore = estimateStore ?? throw new ArgumentNullException(nameof(estimateStore));
        this.mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task<EstimateVm> Handle(EstimateReadQuery query, CancellationToken token = default)
    {
        if (this.queryValidator.IsValid(query) == false)
        {
            var logRecord = new { message = "Query is not valid", query };
            
            this.logger.LogInformation("{Record}", JsonSerializer.Serialize(logRecord));

            throw new ValidationException($"Query is not valid. InvoiceKey={query.InvoiceKey}");
        }
        
        var estimate = await this.estimateStore.FindOrDefault(EstimateFilter.ByInvoiceKey(query.InvoiceKey), token);

        if (estimate is null)
        {
            throw new NotFoundException($"Estimate not found. InvoiceKey={query.InvoiceKey}");
        }

        return this.mapper.Map<EstimateVm>(estimate);
    }
}