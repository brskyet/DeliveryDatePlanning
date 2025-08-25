using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Data;

namespace DeliveryDatePlanning.Data.Model.Apt;

public class Client : LegacyTable
{
    public string CompanyName { get; set; }
}

public class ClientConfiguration : LegacyTableConfiguration<Client>
{
    public override void Configure(EntityTypeBuilder<Client> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Clients");
        
        builder.Property(e => e.OwnerId).HasColumnName("Owner_ID");
        builder.Property(e => e.CompanyName).HasColumnName("Company_name");
    }
}