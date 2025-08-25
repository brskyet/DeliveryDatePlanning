using MongoDB.Driver;
using DeliveryDatePlanning.Data.Model.EstimateDb;

namespace DeliveryDatePlanning.Data.Context;

public class EstimateDbContext
{
    private readonly IMongoDatabase DataBase;
    
    public readonly IMongoCollection<Estimate> EstimateCollection;
    
    public EstimateDbContext(IMongoDatabase dataBase)
    {
        this.DataBase = dataBase ?? throw new ArgumentNullException(nameof(dataBase));
        
        this.EstimateCollection = this.DataBase.GetCollection<Estimate>("estimate_results");
    }
}