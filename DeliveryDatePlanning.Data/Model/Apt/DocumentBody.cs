using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Data;

namespace DeliveryDatePlanning.Data.Model.Apt;

public class DocumentBody : LegacyTable
{
    public int TitleId { get; set; }
    public int TitleOwnerId { get; set; }
    public int? ObjectId { get; set; }
    public int? ObjectOwnerId { get; set; }
    public DocumentTitle DocumentTitle { get; set; }
    public Invoice Invoice { get; set; }
}

public class DocumentBodyConfiguration : LegacyTableConfiguration<DocumentBody>
{
    public override void Configure(EntityTypeBuilder<DocumentBody> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("DocumentBody");
        
        builder.Property(e => e.ObjectId).HasColumnName("Object_Id");
        builder.Property(e => e.ObjectOwnerId).HasColumnName("Object_Owner_Id");
        builder.Property(e => e.TitleId).HasColumnName("Title_Id");
        builder.Property(e => e.TitleOwnerId).HasColumnName("Title_Owner_Id");
        
        builder.HasOne(db => db.DocumentTitle)
            .WithOne(dt => dt.DocumentBody)
            .HasForeignKey<DocumentBody>(i => new { i.TitleId, i.TitleOwnerId})
            .HasPrincipalKey<DocumentTitle>(dt => new {dt.Id, dt.OwnerId});

        builder.HasOne(i => i.Invoice)
            .WithOne(i => i.DocumentBody)
            .HasForeignKey<DocumentBody>(i => new {i.ObjectId, i.ObjectOwnerId})
            .HasPrincipalKey<Invoice>(i => new {i.Id, i.OwnerId});
    }
}