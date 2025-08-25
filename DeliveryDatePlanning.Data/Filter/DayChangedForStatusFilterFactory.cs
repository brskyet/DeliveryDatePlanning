using MongoDB.Driver;
using Models;
using DeliveryDatePlanning.Data.Model.EstimateDb;
using DeliveryDatePlanning.Domain.Enums;

namespace DeliveryDatePlanning.Data.Filter;

public class DayChangedForStatusFilterFactory
{
    private readonly IReadOnlyCollection<EncloseState> states = new[]
    {
        EncloseState.AcceptedByRegistry, EncloseState.Registered, EncloseState.FormedForPassingToLogistician, EncloseState.AcceptedByLogistician, 
        EncloseState.OnLogisticianKladovka, EncloseState.OnRoute, EncloseState.OnCourier, EncloseState.PassedForOutOfCityDelivery, 
        EncloseState.AcceptedForOutOfCityDelivery, EncloseState.BaledForOutOfCityDelivery, EncloseState.CourierAssigned, 
        EncloseState.Consolidated, EncloseState.Searched, EncloseState.Lost, EncloseState.ScannedForDelivery, EncloseState.ScannedToConsolidate, 
        EncloseState.ScannedToRegistry, EncloseState.PassedFictionally, EncloseState.PassedToInIntraregionalRoute, EncloseState.OnCourierConsolidated
    };
    
    private readonly IReadOnlyCollection<EstimateStatusType> statuses = new[]
    {
        EstimateStatusType.NotReached, EstimateStatusType.Reached
    };

    public FilterDefinition<Estimate> Create(string cityTo)
    {
        var inStatesFilter = Builders<Enclose>.Filter.In(x => x.State, states);
        
        var filter = Builders<Estimate>.Filter.Eq(x => x.Invoice.CityTo, cityTo) &
                     Builders<Estimate>.Filter.Ne(x => x.DateStart, null) &
                     Builders<Estimate>.Filter.Ne(x => x.DateEnd, null) &
                     Builders<Estimate>.Filter.In(x => x.Status, statuses) &
                     Builders<Estimate>.Filter.ElemMatch(x => x.Invoice.Encloses, inStatesFilter);

        return filter;
    }
}