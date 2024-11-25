using Microsoft.EntityFrameworkCore;
using WalletService.Domain.Models;

namespace WalletService.Infrastructure.DataContext;

public interface IWalletServiceContext
{
    DbSet<Wallet> Wallets { get; set; }

    Task SaveChangesAsync();
}