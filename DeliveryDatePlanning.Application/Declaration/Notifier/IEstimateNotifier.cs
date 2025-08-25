using Microsoft.Extensions.Logging;
using DeliveryDatePlanning.Domain.Core.Entity;

namespace DeliveryDatePlanning.Application.Declaration.Notifier;

public interface IEstimateResultNotifier
{
    Task Notify(Estimate payload, CancellationToken token = default);
}

public class DummyEstimateResultNotifier : IEstimateResultNotifier
{
    private readonly ILogger<DummyEstimateResultNotifier> logger;

    public DummyEstimateResultNotifier(ILogger<DummyEstimateResultNotifier> logger)
    {
        this.logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    public Task Notify(Estimate payload, CancellationToken token = default)
    {
        this.logger.LogWarning($"You are using dummy instance of ({nameof(IEstimateResultNotifier)})");
        
        return Task.CompletedTask;
    }
}