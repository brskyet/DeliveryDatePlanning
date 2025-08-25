using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Data;

namespace DeliveryDatePlanning.Data.Model.Apt;

public class InvoiceDeliveryPoint : LegacyTable
{
    public int InvoiceId { get; set; }
    public int InvoiceOwnerId { get; set; }
    public int DeliveryPointId { get; set; }
    public int DeliveryPointOwnerId { get; set; }
    public int? FromDeliveryPointId { get; set; }
    public int? FromDeliveryPointOwnerId { get; set; }
    public DeliveryPoint DeliveryPoint { get; set; }
    public DeliveryPoint SourceDeliveryPoint { get; set; }
}

public class InvoiceDeliveryPointConfiguration : LegacyTableConfiguration<InvoiceDeliveryPoint>
{
    public override void Configure(EntityTypeBuilder<InvoiceDeliveryPoint> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Invoice_PTInfo");
        
        builder.Property(e => e.InvoiceId).HasColumnName("Invoice_Id");
        builder.Property(e => e.InvoiceOwnerId).HasColumnName("Invoice_Owner_Id");
        builder.Property(e => e.DeliveryPointId).HasColumnName("PTInfo_Id");
        builder.Property(e => e.DeliveryPointOwnerId).HasColumnName("PTInfo_Owner_Id");
        builder.Property(e => e.FromDeliveryPointId).HasColumnName("PutPTInfo_Id");
        builder.Property(e => e.FromDeliveryPointOwnerId).HasColumnName("PutPTInfo_owner_id");
        

        builder.HasOne(ipd => ipd.DeliveryPoint)
            .WithOne()
            .HasForeignKey<InvoiceDeliveryPoint>(idp => new {idp.DeliveryPointId, idp.DeliveryPointOwnerId})
            .HasPrincipalKey<DeliveryPoint>(dp => new {dp.Id, dp.OwnerId});
        
        builder.HasOne(idp => idp.SourceDeliveryPoint)
            .WithOne()
            .HasForeignKey<InvoiceDeliveryPoint>(idp => new { idp.FromDeliveryPointId, idp.FromDeliveryPointOwnerId })
            .HasPrincipalKey<DeliveryPoint>(dp => new {dp.Id, dp.OwnerId});
    }
}