using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Models.Data;

namespace DeliveryDatePlanning.Data.Model.Apt;

public class Contract : LegacyTable
{
    public string NumberContract { get; set; }
    public List<Invoice> Invoices { get; set; }
}

public class ContractConfiguration : LegacyTableConfiguration<Contract>
{
    public override void Configure(EntityTypeBuilder<Contract> builder)
    {
        base.Configure(builder);
        
        builder.ToTable("Contracts");
    }
}