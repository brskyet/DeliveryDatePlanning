using DeliveryDatePlanning.Domain.Core.ValueObject.Estimate;
using Models;

namespace DeliveryDatePlanning.Domain.Core.Entity;

using CSharpFunctionalExtensions;

public class Estimate : Entity<string>
{
    public DeliveryDates DeliveryDates { get; private set; }

    public EstimateStatusTypeEnum Status { get; private set; }

    public DateChangeReasonTypeEnum Reason { get; private set; }

    public OverdueDays Overdue { get; private set; }

    public Invoice Invoice { get; private set; }

    private Estimate(
        string id, 
        DeliveryDates deliveryDates,
        EstimateStatusTypeEnum status, 
        DateChangeReasonTypeEnum reason, 
        OverdueDays overdue, 
        Invoice invoice
        ) : base(id)
    {
        DeliveryDates = deliveryDates;
        Status = status;
        Reason = reason;
        Overdue = overdue;
        Invoice = invoice;
    }

    public static Result<Estimate> Create(
        string id, 
        DeliveryDates deliveryDates,
        EstimateStatusTypeEnum status, 
        DateChangeReasonTypeEnum reason, 
        OverdueDays overdue, 
        Invoice invoice
        )
    {
        if (string.IsNullOrWhiteSpace(id))
        {
            return Result.Failure<Estimate>($"{nameof(id)} argument is null or whitespace");
        }

        var entity = new Estimate(id, deliveryDates, status, reason, overdue, invoice);

        return entity;
    }
    
    public static Result<Estimate> Create(Estimate src)
    {
        var id = src.Id;
        var deliveryDates = DeliveryDates.Create(src.DeliveryDates.DateStart, src.DeliveryDates.DateEnd).Value;
        var status = EstimateStatusTypeEnum.Create(src.Status).Value;
        var reason = DateChangeReasonTypeEnum.Create(src.Reason).Value;
        var overdue = OverdueDays.Create(src.Overdue).Value;
        var invoice = Invoice.Create(src.Invoice).Value;

        var entity = new Estimate(id, deliveryDates, status, reason, overdue, invoice);

        return entity;
    }

    public static Result<Estimate> CreateDefault(Invoice invoice)
    {
        var id = Guid.NewGuid().ToString();
        var deliveryDates = DeliveryDates.CreateDefault().Value;
        var status = EstimateStatusTypeEnum.CreateDefault().Value;
        var reason = DateChangeReasonTypeEnum.CreateDefault().Value;
        var overdue = OverdueDays.CreateDefault().Value;

        var entity = new Estimate(id, deliveryDates, status, reason, overdue, invoice);

        return entity;
    }

    public Estimate SetDeliveryDates(DeliveryDates value)
    {
        this.DeliveryDates = value;
        
        return this;
    }

    public Estimate SetInvoice(Invoice value)
    {
        this.Invoice = value;
        
        return this;
    }
    
    public override bool Equals(object obj)
    {
        var entity = obj as Estimate;
        
        if ((object) entity == null)
            return false;
        
        if ((object) this == (object) entity)
            return true;

        return this.Id == entity.Id && 
               this.DeliveryDates == entity.DeliveryDates &&
               this.Status == entity.Status && 
               this.Reason == entity.Reason &&
               this.Overdue == entity.Overdue &&
               this.Invoice == entity.Invoice;
    }

    public bool IsDeliveryDateEmpty()
    {
        return DeliveryDates.IsEmpty();
    }

    public Estimate SetStatus(EstimateStatusTypeEnum value)
    {
        this.Status = value;
        
        return this;
    }
    
    public Estimate SetOverdue(OverdueDays value)
    {
        this.Overdue = value;
        
        return this;
    }

    public Estimate SetDateChangeReason(DateChangeReasonTypeEnum value)
    {
        this.Reason = value;

        return this;
    }
    
    public Estimate AddEncloseStateRecord(string key, DateTime timestamp, EncloseState state)
    {
        foreach (var enclose in this.Invoice.Encloses)
        {
            if (enclose.Id == key)
            {
                enclose.AddEncloseStateRecord(timestamp, state).SetState(state);
            }
        }
        
        return this;
    }
}