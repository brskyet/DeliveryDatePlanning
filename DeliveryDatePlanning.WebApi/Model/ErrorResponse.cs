namespace DeliveryDatePlanning.WebApi.Model;

/// <summary>
/// Модель представления данных об ошибке
/// </summary>
public class ErrorResponse
{
    /// <summary>
    /// Текст сообщения об ошибке
    /// </summary>
    public string Error { get; set; }
}