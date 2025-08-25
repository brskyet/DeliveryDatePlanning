using System.Text.RegularExpressions;
using CSharpFunctionalExtensions;
using DeliveryDatePlanning.Domain.Core.ValueObject.Invoice;

namespace DeliveryDatePlanning.Domain.Core.Entity;

/// <summary>
/// Связанное отправление (для возвратов)
/// </summary>
public class LinkedInvoice : Entity<string>
{
    public PostageTypeEnum PostageType { get; }
    public  Enclose[] Encloses  { get; }
    public DeliveryPointJurName RecipientPointJurName { get; } 
    
    
    private LinkedInvoice(
        string id,
        PostageTypeEnum postageType,
        Enclose[]  encloses,
        DeliveryPointJurName recipientPointJurName
    ) : base(id)
    {
        this.PostageType = postageType;
        this.Encloses = encloses;
        this.RecipientPointJurName = recipientPointJurName;
    }

    public static Result<LinkedInvoice> Create(
        string id,
        PostageTypeEnum postageType,
        Enclose[] encloses,
        DeliveryPointJurName recipientPointJurName
    )
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return Result.Failure<LinkedInvoice>($"{nameof(id)} argument is null or whitespace");
        }

        if (Regex.IsMatch(id, @"(^[0-9]+)[-]([0-9]+$)") == false)
        {
            return Result.Failure<LinkedInvoice>($"{nameof(id)} format is invalid");
        }

        if (encloses is null || encloses.Length == 0)
        {
            return Result.Failure<LinkedInvoice>($"{nameof(encloses)} count must be greater than 0");
        }

        var entity = new LinkedInvoice(id, postageType, encloses, recipientPointJurName);

        return entity;
    }
}