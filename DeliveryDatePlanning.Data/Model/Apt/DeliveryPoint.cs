using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Data;

namespace DeliveryDatePlanning.Data.Model.Apt;

public class DeliveryPoint : LegacyTable
{
    public string Number { get; set; }
    public int? AddressId { get; set; }
    public int? AddressOwnerId { get; set; }
    public ClientDeliveryPoint ClientDeliveryPoint { get; set; }
    public Address Address { get; set; }
}

public class DeliveryPointConfiguration : LegacyTableConfiguration<DeliveryPoint>
{
    public override void Configure(EntityTypeBuilder<DeliveryPoint> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("PTInfo");
        
        builder.Property(e => e.Number).HasColumnName("PTNumber");  
        builder.Property(e => e.AddressId).HasColumnName("Address_id");  
        builder.Property(e => e.AddressOwnerId).HasColumnName("Address_Owner_Id");  

        builder.HasOne(dp => dp.Address)
            .WithOne()
            .HasForeignKey<DeliveryPoint>(dp => new {dp.AddressId, dp.AddressOwnerId})
            .HasPrincipalKey<Address>(a => new {a.Id, a.OwnerId});
        
        builder.HasOne(dp => dp.ClientDeliveryPoint)
            .WithOne()
            .HasForeignKey<DeliveryPoint>(dp => new {dp.Id, dp.OwnerId})
            .HasPrincipalKey<ClientDeliveryPoint>(cdp => new {cdp.DeliveryPointId, cdp.DeliveryPointOwnerId});
    }
}