namespace DeliveryDatePlanning.Application.Common.Exception;

public class ValidationException : System.Exception
{
    public ValidationException(string message) : base(message)
    {
    }
}