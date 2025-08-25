using System.ComponentModel;

namespace DeliveryDatePlanning.Domain.Enums;

/// <summary>
/// Причина сдвига расчетного срока доставки
/// </summary>
public enum DateChangeReasonType
{
    /// <summary>
    /// (-), значение по-умолчанию
    /// </summary>
    [Description("-")]
    None = 0,
    /// <summary>
    /// Изменена дата приема
    /// </summary>
    [Description("Изменена дата приема")]
    ReceptionDateChanged,
    /// <summary>
    /// Изменен город отправителя
    /// </summary>
    [Description("Изменен город отправителя")]
    SenderCityChanged,
    /// <summary>
    /// Изменен город получателя
    /// </summary>
    [Description("Изменен город получателя")]
    RecipientCityChanged,
    /// <summary>
    /// Переадресовано
    /// </summary>
    [Description("Переадресовано")]
    Forwarded,
    /// <summary>
    /// Клиент не сдал груз
    /// </summary>
    [Description("Клиент не сдал груз")]
    ItemsNotProvidedByClient
}