using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Data;

namespace DeliveryDatePlanning.Data.Model.Apt;

public class EncloseState : LegacyTable
{
    public int EncloseId { get; set; }
    public int EncloseOwnerId { get; set; }
    public int? SubState { get; set; }
    public int? NotUse { get; set; }
    public DateTime ChangeDateTime { get; set; }
    public Models.EncloseState State { get; set; }
    public Enclose Enclose { get; set; }
}

public class EncloseStateConfiguration : LegacyTableConfiguration<EncloseState>
{
    public override void Configure(EntityTypeBuilder<EncloseState> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("EncloseStates");
        
        builder.Property(e => e.EncloseId).HasColumnName("enclose_id");            
        builder.Property(e => e.EncloseOwnerId).HasColumnName("enclose_owner_id");
        builder.Property(e => e.SubState).HasColumnName("SubState");
        builder.Property(e => e.State).HasColumnName("EncloseState");
        builder.Property(e => e.ChangeDateTime).HasColumnName("ChangeDT");

        builder.HasOne(es => es.Enclose)
            .WithMany(e => e.EncloseStates)
            .HasForeignKey(es => new { es.EncloseId, es.EncloseOwnerId })
            .HasPrincipalKey(e => new {e.Id, e.OwnerId});
    }
}