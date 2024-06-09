using EnergyPortalApi.Data.Models;
using Microsoft.EntityFrameworkCore;

namespace EnergyPortalApi.Data.Context;

internal class AccountContext(DbContextOptions<AccountContext> options) : DbContext(options)
{
    internal DbSet<DbAccount> Accounts { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<DbAccount>()
            .ToTable("Accounts", "EnergyPortal")
            .HasKey(entity => entity.Id);
    }
}
