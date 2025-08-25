using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Data;

namespace DeliveryDatePlanning.Data.Model.Apt;

public class DocumentTitle : LegacyTable
{
    public Invoice Invoice { get; set; }
    public DocumentBody DocumentBody { get; set; }
}

public class DocumentTitleConfiguration : LegacyTableConfiguration<DocumentTitle>
{
    public override void Configure(EntityTypeBuilder<DocumentTitle> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("DocumentTitle");
    }
}