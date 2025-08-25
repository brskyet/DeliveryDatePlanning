using DeliveryDatePlanning.Application.Declaration.Estimator;
using DeliveryDatePlanning.Application.Declaration.Handler.Command;

namespace DeliveryDatePlanning.Application.Declaration.Handler;

public interface ICommandHandler
{
    Task Handle(EstimateCommand command, IEstimationStrategy strategy, CancellationToken token = default);
    
    Task Handle(EncloseStateRecordCommand command, CancellationToken token = default);
    
    Task Handle(DayChangedPublishingCommand command, CancellationToken token = default);

    Task Handle(InvoiceDataChangedCommand command, CancellationToken token = default);
}