using MongoDB.Driver;
using Models;
using DeliveryDatePlanning.Data.Model.EstimateDb;

namespace DeliveryDatePlanning.Data.Filter;

public class DayChangedForNotAcceptedFilterFactory
{
    private readonly IReadOnlyCollection<EncloseState> states = new[]
    {
        EncloseState.Registered, EncloseState.FormedForPassingToLogistician, EncloseState.WaitingForClientDeliveryToPT, EncloseState.ReadyForSending
    };
    
    public FilterDefinition<Estimate> Create(string cityTo)
    {
        var inStatesFilter = Builders<Enclose>.Filter.In(x => x.State, states);
        
        var filter = Builders<Estimate>.Filter.Eq(x => x.Invoice.CityTo, cityTo) &
                     Builders<Estimate>.Filter.Ne(x => x.DateStart, null) &
                     Builders<Estimate>.Filter.Ne(x => x.DateEnd, null) &
                     Builders<Estimate>.Filter.ElemMatch(x => x.Invoice.Encloses, inStatesFilter);

        return filter;
    }
}