using LeaseCrunch.Domain;
using Microsoft.EntityFrameworkCore;

namespace LeaseCrunch.Storage;

public class LeaseDbContext : DbContext
{
    public DbSet<LeaseData> Leases { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseInMemoryDatabase("LeaseCrunch");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LeaseData>().HasKey(l => l.Name);

        base.OnModelCreating(modelBuilder);
    }
}