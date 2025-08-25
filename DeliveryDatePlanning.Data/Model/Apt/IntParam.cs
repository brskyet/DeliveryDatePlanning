using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeliveryDatePlanning.Data.Model.Apt
{
    public class IntParam : LegacyTable
    {
        public int? ObjectId { get; set; }

        public int? ObjectOwnerId { get; set; }

        public int? ParameterType { get; set; }

        public int? Value { get; set; }

        public Invoice Invoice { get; set; }
    }

    public class IntParamsConfiguration : LegacyTableConfiguration<IntParam>
    {
        public override void Configure(EntityTypeBuilder<IntParam> model)
        {
            base.Configure(model);
            model.ToTable("ExtParams_Int");
            model.Property(m => m.ObjectId).HasColumnName("Object_id");
            model.Property(m => m.ObjectOwnerId).HasColumnName("object_owner_id");
            model.Property(m => m.ParameterType).HasColumnName("PType");

            model.HasOne(m => m.Invoice)
                .WithMany(c => c.IntParams)
                .HasPrincipalKey(i => new { i.Id, i.OwnerId })
                .HasForeignKey(m => new { m.ObjectId, m.ObjectOwnerId });

        }
    }
}
