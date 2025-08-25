using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models;
using Models.Data;

namespace DeliveryDatePlanning.Data.Model.Apt;

public class Invoice : LegacyTable
{
    public int ReceiptAddressId { get; set; }
    public int ReceiptAddressOwnerId { get; set; }
    public int OrigAddressId { get; set; }
    public int OrigAddressOwnerId { get; set; }
    public int DocumentTitleId { get; set; }
    public int DocumentTitleOwnerId { get; set; }
    public int ContractId { get; set; }
    public int ContractOwnerId { get; set; }
    public DateTime? ReceptionDate { get; set; }
    public PostageType? PostageType { get; set; }
    public GettingType? GettingType { get; set; }
    public List<Enclose> Encloses { get; set; }
    public Contract Contract { get; set; }
    public InvoiceDeliveryPoint InvoiceDeliveryPoint { get; set; }
    public Address DestinationAddress { get; set; }
    public Address SourceAddress { get; set; }
    public InvoiceAttribute InvoiceAttribute { get; set; }
    public DocumentTitle DocumentTitle { get; set; }
    public DocumentBody DocumentBody { get; set; }
    public List<IntParam> IntParams { get; set; }
}

public class InvoiceConfiguration : LegacyTableConfiguration<Invoice>
{
    public override void Configure(EntityTypeBuilder<Invoice> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Invoice");
        
        builder.Property(e => e.ContractId).HasColumnName("Dogovor_ID");
        builder.Property(e => e.ContractOwnerId).HasColumnName("Dogovor_Owner_Id");
        builder.Property(e => e.PostageType).HasColumnName("Postage_Type");
        builder.Property(e => e.GettingType).HasColumnName("Getting_Type");
        builder.Property(e => e.DocumentTitleId).HasColumnName("DocumentTitle_Id");
        builder.Property(e => e.DocumentTitleOwnerId).HasColumnName("DocumentTitle_Owner_Id");
        builder.Property(e => e.ReceiptAddressId).HasColumnName("Receipt_Address_Id");
        builder.Property(e => e.ReceiptAddressOwnerId).HasColumnName("Receipt_Address_Owner_Id");
        builder.Property(e => e.OrigAddressId).HasColumnName("Orig_Address_Id");
        builder.Property(e => e.OrigAddressOwnerId).HasColumnName("Orig_Address_Owner_Id");
        builder.Property(e => e.ReceptionDate).HasColumnName("Receipt_Date");

        builder.HasOne(i => i.Contract)
            .WithMany(c => c.Invoices)
            .HasForeignKey(i => new { i.ContractId, i.ContractOwnerId })
            .HasPrincipalKey(c => new {c.Id, c.OwnerId});
       
        builder.HasOne(i => i.DocumentTitle)
            .WithOne(dt => dt.Invoice)
            .HasForeignKey<Invoice>(i => new { i.DocumentTitleId, i.DocumentTitleOwnerId})
            .HasPrincipalKey<DocumentTitle>(dt => new {dt.Id, dt.OwnerId});
        
        builder.HasOne(i => i.DestinationAddress)
            .WithMany()
            .HasForeignKey(i => new { i.ReceiptAddressId, i.ReceiptAddressOwnerId })
            .HasPrincipalKey(da => new {da.Id, da.OwnerId});

        builder.HasOne(i => i.SourceAddress)
            .WithMany()
            .HasForeignKey(i => new { i.OrigAddressId, i.OrigAddressOwnerId })
            .HasPrincipalKey(a => new {a.Id, a.OwnerId});
        
        builder.HasOne(i => i.InvoiceDeliveryPoint)
            .WithOne()
            .HasForeignKey<InvoiceDeliveryPoint>(idp => new {idp.InvoiceId, idp.InvoiceOwnerId})
            .HasPrincipalKey<Invoice>(i => new {i.Id, i.OwnerId});

        builder.HasMany(i => i.IntParams)
            .WithOne(s => s.Invoice)
            .HasForeignKey(s => new { s.ObjectId, s.ObjectOwnerId })
            .HasPrincipalKey(i => new { i.Id, i.OwnerId });
    }
}