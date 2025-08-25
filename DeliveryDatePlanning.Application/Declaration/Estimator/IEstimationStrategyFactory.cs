namespace DeliveryDatePlanning.Application.Declaration.Estimator;

public interface IEstimationStrategyFactory
{
    IEstimationStrategy OnlyStatus();
    
    IEstimationStrategy DateAndStatus();
}