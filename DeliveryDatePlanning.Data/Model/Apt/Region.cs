using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Data;

namespace DeliveryDatePlanning.Data.Model.Apt;

public class Region : LegacyTable
{
    public int TimeZone { get; set; }
}

public class RegionConfiguration : LegacyTableConfiguration<Region>
{
    public override void Configure(EntityTypeBuilder<Region> model)
    {
        base.Configure(model);

        model.ToTable("Regions");
    }
}