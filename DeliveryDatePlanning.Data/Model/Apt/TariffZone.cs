using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Data;

namespace DeliveryDatePlanning.Data.Model.Apt;

public class TariffZone : LegacyTable
{
    public string Comment { get; set; }
}

public class TariffZoneConfiguration : LegacyTableConfiguration<TariffZone>
{
    public override void Configure(EntityTypeBuilder<TariffZone> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Tarif_Zones");
        
        builder.Property(e => e.OwnerId).HasColumnName("Owner_Id");
        builder.Property(t => t.CompanyLegacyState).HasColumnName("State");
    }
}