using AzureAppConfigurationEmulator.Entities;
using Microsoft.EntityFrameworkCore;

namespace AzureAppConfigurationEmulator.Contexts;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<ConfigurationSetting> ConfigurationSettings => Set<ConfigurationSetting>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<ConfigurationSetting>(typeBuilder =>
        {
            typeBuilder.HasKey(setting => new { setting.Key, setting.Label });
        });
    }
}
