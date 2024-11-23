using WalletService.Domain.Models;

namespace WalletService.Infrastructure.Repository.Interfaces;

public interface IWalletRepository
{
    /// <summary>
    /// Add a wallet to the database
    /// </summary>
    /// <returns>An integer representing the added Wallet's Id</returns>
    Task<int> AddWallet(Wallet wallet);
}
