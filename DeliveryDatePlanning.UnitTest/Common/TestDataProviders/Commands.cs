using DeliveryDatePlanning.Application.Declaration.Handler.Command;

namespace DeliveryDatePlanning.UnitTest.Common.TestDataProviders;

public static class Commands
{
    public static EstimateCommand CommandsFactory(string exchangeName)
    {
        return new EstimateCommand(DateTime.Now, exchangeName).SetEnclose("123-456");
    }
}