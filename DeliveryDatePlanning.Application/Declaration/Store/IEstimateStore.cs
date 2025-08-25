using DeliveryDatePlanning.Domain.Core.Entity;

namespace DeliveryDatePlanning.Application.Declaration.Store;

public interface IEstimateStore
{
    Task<Estimate> FindOrDefault(EstimateFilter filter, CancellationToken token = default);

    Task Upsert(Estimate entity, CancellationToken token = default);
    
    Task UpdateEncloseStateRecord(Estimate entity, CancellationToken token = default);
    
    Task UpdateReceptionDate(Estimate entity, CancellationToken token = default);

    Task<string[]> RelevantDayChangedForStatus(string cityTo, int limit = default, CancellationToken token = default);
    
    Task<string[]> RelevantDayChangedForOverdue(string cityTo, int limit = default, CancellationToken token = default);
    
    Task<string[]> RelevantDayChangedForNotAccepted(string cityTo, int limit = default, CancellationToken token = default);
    
    Task UpdateInvoice(EstimateFilter filter, Invoice invoice, CancellationToken token = default);
}