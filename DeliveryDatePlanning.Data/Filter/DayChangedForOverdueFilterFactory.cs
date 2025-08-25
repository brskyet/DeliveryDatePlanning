using MongoDB.Driver;
using Models;
using DeliveryDatePlanning.Data.Model.EstimateDb;
using DeliveryDatePlanning.Domain.Enums;

namespace DeliveryDatePlanning.Data.Filter;

public class DayChangedForOverdueFilterFactory
{
    private readonly IReadOnlyCollection<EncloseState> states = new[]
    {
        EncloseState.DeliveredToPT, EncloseState.Received, EncloseState.ReturnedToClient, EncloseState.Rejected, EncloseState.Annulled
    };
    
    private readonly IReadOnlyCollection<EstimateStatusType> statuses = new[]
    {
        EstimateStatusType.Expired
    };
    
    public FilterDefinition<Estimate> Create(string cityTo)
    {
        var notInStatesFilter = Builders<Enclose>.Filter.Nin(x => x.State, states);

        var filter = Builders<Estimate>.Filter.Eq(x => x.Invoice.CityTo, cityTo) &
                     Builders<Estimate>.Filter.Ne(x => x.DateStart, null) &
                     Builders<Estimate>.Filter.Ne(x => x.DateEnd, null) &
                     Builders<Estimate>.Filter.In(x => x.Status, statuses) &
                     Builders<Estimate>.Filter.ElemMatch(x => x.Invoice.Encloses, notInStatesFilter);

        return filter;
    }
}