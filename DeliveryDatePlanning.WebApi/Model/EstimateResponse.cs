using AutoMapper;
using DeliveryDatePlanning.Application.Common.Mapping;
using DeliveryDatePlanning.Application.Declaration.Handler.Query;
using DeliveryDatePlanning.Domain.Enums;

namespace DeliveryDatePlanning.WebApi.Model;

/// <summary>
/// Модель представления данных РСД в ответе метода API
/// </summary>
public class EstimateResponse : IMapWith<EstimateVm>
{
    /// <summary>
    /// РСД. Начальная дата
    /// </summary>
    public string DateStart { get; set; }
    /// <summary>
    /// РСД. Конечная дата
    /// </summary>
    public string DateEnd { get; set; }
    /// <summary>
    /// РСД. Статус
    /// </summary>
    public EstimateStatusType Status { get; set; }
    /// <summary>
    /// Причина сдвига расчетного срока доставки
    /// </summary>
    public DateChangeReasonType Reason { get; set; }
    /// <summary>
    /// Количество дней просрочки
    /// </summary>
    public int? Overdue { get; set; }
    
    public void Mapping(Profile profile)
    {
        profile.CreateMap<EstimateVm, EstimateResponse>();
    }
}