using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using DeliveryDatePlanning.Data.Index;

namespace DeliveryDatePlanning.BackgroundService;

public class IndexHostedService : IHostedService
{
    private readonly MongoDbIndex mongoDbIndex;
    private readonly ILogger<IndexHostedService> logger;

    public IndexHostedService(MongoDbIndex mongoDbIndex, ILogger<IndexHostedService> logger)
    {
        this.mongoDbIndex = mongoDbIndex ?? throw new ArgumentNullException(nameof(mongoDbIndex));
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("{Record}", JsonSerializer.Serialize(new { message = "Index creation for MongoDB begin" }));
        
        var result = await this.mongoDbIndex.CreateAsync(cancellationToken);

        this.logger.LogInformation("{Record}", JsonSerializer.Serialize(new { message = "Index were created for MongoDB", result }));
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        this.logger.LogInformation("{Record}", JsonSerializer.Serialize(new { message = "Index service were stopped" }));
        
        return Task.CompletedTask;
    }
}