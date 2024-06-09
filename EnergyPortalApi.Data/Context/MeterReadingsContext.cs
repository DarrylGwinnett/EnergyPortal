using EnergyPortalApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergyPortalApi.Data.Context;

public class MeterReadingsContext(DbContextOptions<MeterReadingsContext> options) : DbContext(options)
{
    internal DbSet<DbMeterReading> MeterReadings { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbMeterReading>()
            .ToTable("MeterReadings", "EnergyPortal")
            .HasKey(entity => entity.Id);
    }
}
