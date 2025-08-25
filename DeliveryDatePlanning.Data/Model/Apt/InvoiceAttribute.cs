using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Data;

namespace DeliveryDatePlanning.Data.Model.Apt;

public class InvoiceAttribute : LegacyTable
{
    public int InvoiceId { get; set; }
    public int InvoiceOwnerId { get; set; }
    public int? DeliveryMode { get; set; }
    public Invoice Invoice { get; set; }
}

public class InvoiceAttributeConfiguration : LegacyTableConfiguration<InvoiceAttribute>
{
    public override void Configure(EntityTypeBuilder<InvoiceAttribute> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("InvoiceAttributes");

        builder.Property(e => e.InvoiceId).HasColumnName("invoice_id");
        builder.Property(e => e.InvoiceOwnerId).HasColumnName("invoice_owner_id");

        builder.HasOne(ia => ia.Invoice)
            .WithOne(i => i.InvoiceAttribute)
            .HasForeignKey<InvoiceAttribute>(ia => new {ia.InvoiceId, ia.InvoiceOwnerId})
            .HasPrincipalKey<Invoice>(i => new {i.Id, i.OwnerId});
    }
}