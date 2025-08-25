using System.ComponentModel;

namespace DeliveryDatePlanning.Domain.Enums;

/// <summary>
/// Перечисление статусов РСД
/// </summary>
public enum EstimateStatusType
{
    /// <summary>
    /// (-), значение по-умолчанию
    /// </summary>
    [Description("-")]
    None = 0,
    /// <summary>
    /// Не наступил
    /// </summary>
    [Description("Не наступил")]
    NotReached,
    /// <summary>
    /// Не наступил - доставлено
    /// </summary>
    [Description("Не наступил - доставлено")]
    NotReachedDelivered,
    /// <summary>
    /// Не наступил - отказ
    /// </summary>
    [Description("Не наступил - отказ")]
    NotReachedRejected,
    /// <summary>
    /// Наступил
    /// </summary>
    [Description("Наступил")]
    Reached,
    /// <summary>
    /// Просрочен
    /// </summary>
    [Description("Просрочен")]
    Expired
}