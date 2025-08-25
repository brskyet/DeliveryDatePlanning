using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Data;

namespace DeliveryDatePlanning.Data.Model.Apt;

public class Enclose : LegacyTable
{
    public int InvoiceId { get; set; }
    public int InvoiceOwnerId { get; set; }
    public string BarCode { get; set; }
    public int? EncloseState { get; set; }
    public Invoice Invoice { get; set; }
    public List<EncloseState> EncloseStates { get; set; }
}

public class EncloseConfiguration : LegacyTableConfiguration<Enclose>
{
    public override void Configure(EntityTypeBuilder<Enclose> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Enclose");
        
        builder.Property(e => e.InvoiceId).HasColumnName("Invoice_Id");
        builder.Property(e => e.InvoiceOwnerId).HasColumnName("Invoice_Owner_Id");
        builder.Property(e => e.BarCode).HasColumnName("BarCode");
        builder.Property(e => e.EncloseState).HasColumnName("EncloseState");

        builder.HasOne(e => e.Invoice)
            .WithMany(i => i.Encloses)
            .HasForeignKey(e => new { e.InvoiceId, e.InvoiceOwnerId })
            .HasPrincipalKey(i => new {i.Id, i.OwnerId});
    }
}