using Microsoft.EntityFrameworkCore;
using WalletService.Domain.Models;
using WalletService.Infrastructure.EntityConfigurations;

namespace WalletService.Infrastructure.DataContext;

public class WalletServiceContext(DbContextOptions<WalletServiceContext> dbContextOptions) : DbContext(dbContextOptions), IWalletServiceContext
{
    public DbSet<Wallet> Wallets { get; set; }

    public async Task SaveChangesAsync()
    {
        await base.SaveChangesAsync();
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new WalletEntityConfiguration());
    }
}
