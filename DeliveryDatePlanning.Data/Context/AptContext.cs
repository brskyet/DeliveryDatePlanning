using Microsoft.EntityFrameworkCore;

namespace DeliveryDatePlanning.Data.Context;

using Model.Apt;

public class AptContext : DbContext
{
    public DbSet<TariffZone> TariffZones { get; set; }
    public DbSet<Invoice> Invoices { get; set; }
    public DbSet<Enclose> Encloses { get; set; }
    public DbSet<Contract> Contracts { get; set; }
    public DbSet<InvoiceDeliveryPoint> InvoiceDeliveryPoints { get; set; }
    public DbSet<DeliveryPoint> DeliveryPoints { get; set; }
    public DbSet<ClientDeliveryPoint> ClientDeliveryPoints { get; set; }
    public DbSet<Client> Clients { get; set; }
    public DbSet<EncloseState> EncloseStates { get; set; }
    public DbSet<Address> Addresses { get; set; }
    public DbSet<InvoiceAttribute> InvoiceAttributes { get; set; }
    public DbSet<DocumentTitle> DocumentTitles { get; set; }
    public DbSet<DocumentBody> DocumentBodies { get; set; }
    public DbSet<City> Cities { get; set; }
    public DbSet<IntParam> IntParams { get; set; }

    public AptContext(DbContextOptions<AptContext> context) : base(context)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new InvoiceConfiguration());
        modelBuilder.ApplyConfiguration(new EncloseConfiguration());
        modelBuilder.ApplyConfiguration(new DeliveryPointConfiguration());
        modelBuilder.ApplyConfiguration(new ClientDeliveryPointConfiguration());
        modelBuilder.ApplyConfiguration(new ClientConfiguration());
        modelBuilder.ApplyConfiguration(new ContractConfiguration());
        modelBuilder.ApplyConfiguration(new InvoiceDeliveryPointConfiguration());
        modelBuilder.ApplyConfiguration(new TariffZoneConfiguration());
        modelBuilder.ApplyConfiguration(new EncloseStateConfiguration());
        modelBuilder.ApplyConfiguration(new InvoiceAttributeConfiguration());
        modelBuilder.ApplyConfiguration(new AddressConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentTitleConfiguration());
        modelBuilder.ApplyConfiguration(new DocumentBodyConfiguration());
        modelBuilder.ApplyConfiguration(new CityConfiguration());
        modelBuilder.ApplyConfiguration(new RegionConfiguration());
        modelBuilder.ApplyConfiguration(new IntParamsConfiguration());

        base.OnModelCreating(modelBuilder);
    }
}