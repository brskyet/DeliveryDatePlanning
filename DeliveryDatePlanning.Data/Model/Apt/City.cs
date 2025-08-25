using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Data;

namespace DeliveryDatePlanning.Data.Model.Apt;

public class City : LegacyTable
{
    public int RegionId { get; set; }
    public int RegionOwnerId { get; set; }
    public Region Region { get; set; }
}

public class CityConfiguration : LegacyTableConfiguration<City>
{
    public override void Configure(EntityTypeBuilder<City> model)
    {
        base.Configure(model);
        
        model.Property(m => m.RegionId).HasColumnName("Region_ID");
        model.Property(m => m.RegionOwnerId).HasColumnName("Region_Owner_ID");

        model.HasOne(m => m.Region)
            .WithMany()
            .HasForeignKey(m => new { m.RegionId, m.RegionOwnerId })
            .HasPrincipalKey(r => new {r.Id, r.OwnerId});

        model.ToTable("Cities");
    }
}