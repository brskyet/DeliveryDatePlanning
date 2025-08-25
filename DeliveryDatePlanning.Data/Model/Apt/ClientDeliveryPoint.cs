using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Data;

namespace DeliveryDatePlanning.Data.Model.Apt;

public class ClientDeliveryPoint : LegacyTable
{
    public int DeliveryPointId { get; set; }
    public int DeliveryPointOwnerId { get; set; }
    public int ClientId { get; set; }
    public int ClientOwnerId { get; set; }
    public Client Client { get; set; }
}

public class ClientDeliveryPointConfiguration : LegacyTableConfiguration<ClientDeliveryPoint>
{
    public override void Configure(EntityTypeBuilder<ClientDeliveryPoint> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Client_PTInfo");
        
        builder.Property(e => e.OwnerId).HasColumnName("owner_id");
        builder.Property(e => e.DeliveryPointId).HasColumnName("PTinfo_id");
        builder.Property(e => e.DeliveryPointOwnerId).HasColumnName("PTinfo_owner_id");
        builder.Property(e => e.ClientId).HasColumnName("client_id");
        builder.Property(e => e.ClientOwnerId).HasColumnName("client_owner_id");
        
        builder.HasOne(cdp => cdp.Client)
            .WithOne()
            .HasForeignKey<ClientDeliveryPoint>(cdp => new {cdp.ClientId, cdp.ClientOwnerId})
            .HasPrincipalKey<Client>(c => new {c.Id, c.OwnerId});
    }
}