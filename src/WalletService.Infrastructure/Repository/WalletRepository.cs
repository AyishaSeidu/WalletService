using Microsoft.EntityFrameworkCore;
using WalletService.Domain.Models;
using WalletService.Infrastructure.DataContext;
using WalletService.Infrastructure.Repository.Interfaces;

namespace WalletService.Infrastructure.Repository;

public class WalletRepository(IWalletServiceContext context) : IWalletRepository
{
    /// <inheritdoc />
    public async Task<int> AddWallet(Wallet wallet)
    {
        ArgumentNullException.ThrowIfNull(wallet, nameof(wallet));

        if (await context.Wallets.AnyAsync(x => x.AccountNumber == wallet.AccountNumber && x.Owner == wallet.Owner && x.IsActive)) 
        {
            throw new InvalidOperationException("Wallet already added for user");
        }

        if(await context.Wallets.CountAsync(x=> x.Owner == wallet.Owner && x.IsActive) >= Constants.Constants.MaxNumberOfWalletsPerPerson)
        {
            throw new InvalidOperationException("A user cannot have more than 5 wallets");
        }
        context.Wallets.Add(wallet);
                
        await context.SaveChangesAsync();

        return wallet.Id;
    }
}
