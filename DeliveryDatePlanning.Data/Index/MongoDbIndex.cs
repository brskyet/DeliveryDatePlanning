using MongoDB.Bson;
using MongoDB.Driver;
using DeliveryDatePlanning.Data.Context;
using DeliveryDatePlanning.Data.Model.EstimateDb;

namespace DeliveryDatePlanning.Data.Index;

public class MongoDbIndex
{
    private readonly EstimateDbContext context;

    public MongoDbIndex(EstimateDbContext context)
    {
        this.context = context ?? throw new ArgumentNullException(nameof(context));
    }

    public async Task<IEnumerable<string>> CreateAsync(CancellationToken cancellationToken)
    {
        var invoiceKeyIndexDefinition = new BsonDocument
        {
            {$"{nameof(Estimate.Invoice)}.{nameof(Estimate.Invoice.Key)}", 1}
        };
        
        var invoiceKeyIndexOptions = new CreateIndexOptions { Name = "invoice-key", Unique = true, Background = true };
        
        var encloseKeyIndexDefinition = new BsonDocument
        {
            {$"{nameof(Estimate.Invoice)}.{nameof(Estimate.Invoice.Encloses)}.{nameof(Enclose.Key)}", 1}
        };
        
        var encloseKeyIndexOptions = new CreateIndexOptions { Name = "enclose-key", Background = true };

        var dayChangedIndexDefinition = new BsonDocument
        {
            { $"{nameof(Estimate.Invoice)}.{nameof(Estimate.Invoice.CityTo)}", 1 },
            { $"{nameof(Estimate.DateStart)}", 1 },
            { $"{nameof(Estimate.DateEnd)}", 1 },
            { $"{nameof(Estimate.Status)}", 1 },
            { $"{nameof(Estimate.Invoice)}.{nameof(Estimate.Invoice.Encloses)}.{nameof(Enclose.State)}", 1 },
            { $"{nameof(Estimate.Invoice)}.{nameof(Estimate.Invoice.Key)}", 1 },
        };
        
        var dayChangedIndexOptions = new CreateIndexOptions { Name = "day-changed", Background = true };
        
        var result = await this.context.EstimateCollection.Indexes.CreateManyAsync(
            new[]
            {
                new CreateIndexModel<Estimate>(invoiceKeyIndexDefinition, invoiceKeyIndexOptions),
                new CreateIndexModel<Estimate>(encloseKeyIndexDefinition, encloseKeyIndexOptions),
                new CreateIndexModel<Estimate>(dayChangedIndexDefinition, dayChangedIndexOptions),
                
            }, cancellationToken);

        return result;
    }
}