using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DeliveryDatePlanning.Data.Model.Apt;

public class Address
{
    public int Id { get; set; }
    public int OwnerId { get; set; }
    public int CityId { get; set; }
    public int CityOwnerId { get; set; }
}

public class AddressConfiguration : IEntityTypeConfiguration<Address>
{
    public void Configure(EntityTypeBuilder<Address> builder)
    {
        builder.ToTable("Address");
        
        builder.Property(e => e.OwnerId).HasColumnName("Owner_Id");
        builder.Property(e => e.CityId).HasColumnName("Cities_Id");
        builder.Property(e => e.CityOwnerId).HasColumnName("Cities_Owner_Id");
    }
}