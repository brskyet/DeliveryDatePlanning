using DeliveryDatePlanning.Application.Declaration.Handler.Query;

namespace DeliveryDatePlanning.Application.Declaration.Handler;

public interface IQueryHandler
{
    Task<EstimateVm> Handle(EstimateReadQuery query, CancellationToken token = default);
}