using WalletService.Domain.Models;

namespace WalletService.Infrastructure.Repository.Interfaces;

public interface IWalletRepository
{
    /// <summary>
    /// Add a wallet to the database
    /// </summary>
    /// <returns>The added wallet/returns>
    Task<Wallet> AddWallet(Wallet wallet);

    /// <summary>
    /// Marks the given wallet as deleted 
    /// </summary>
    /// <returns>The deleted wallet</returns>
    Task<Wallet> MarkAsDeleted(int wallet);

    /// <summary>
    /// Retrieves a single given it's Id
    /// </summary>
    Task<Wallet?> GetWalletById(int walletId);

    /// <summary>
    /// Retrieves all active wallets from the database
    /// </summary>
    /// <returns>A list of wallets</returns>
    Task<IEnumerable<Wallet>> GetAllWallets();
}
