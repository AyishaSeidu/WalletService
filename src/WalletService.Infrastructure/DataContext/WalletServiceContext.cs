using Microsoft.EntityFrameworkCore;
using WalletService.Domain.Models;
using WalletService.Infrastructure.EntityConfigurations;

namespace WalletService.Infrastructure.DataContext;

public class WalletServiceContext(DbContextOptions<WalletServiceContext> dbContextOptions) : DbContext(dbContextOptions)
{
    public DbSet<Wallet> Wallets { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfiguration(new WalletEntityConfiguration());
    }
}
