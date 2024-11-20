using WalletService.Domain.Models;
using WalletService.Domain.Models.Enums;
using Xunit;

namespace WalletService.Domain.Tests.Models;

public class WalletTests
{
    [Fact]
    public void Wallet_Create_SetsPropertiesCorrectly()
    {
        // Arrange
        const string walletName = "Ayisha's MTN";
        const string acountNumber = "0246777245";
        const string walletOwner = "0507157798";

        // Act
        var result = new Wallet(walletName, acountNumber, WalletType.MOMO, AccountScheme.VODAFONE, walletOwner);

        // Assert
        Assert.Equal(acountNumber, result.AccountNumber);
        Assert.Equal(walletName, result.WalletName);
        Assert.Equal(walletOwner, result.Owner);
        Assert.Equal(WalletType.MOMO, result.WalletType);
        Assert.Equal(AccountScheme.VODAFONE, result.AccountScheme);
        Assert.True(result.IsActive);
        Assert.Null(result.UpdatedAt);
    }

    [Fact]
    public void Wallet_Deactivate_SetsActiveStatusAndUpdatedAtdates()
    {
        // Arrange
        const string walletName = "Ayisha's MTN";
        const string acountNumber = "0246777245";
        const string walletOwner = "0507157798";


        // Act
        var result = new Wallet(walletName, acountNumber, WalletType.MOMO, AccountScheme.VODAFONE, walletOwner);
        result.DeactivateWallet();

        // Assert
        Assert.Equal(acountNumber, result.AccountNumber);
        Assert.Equal(walletName, result.WalletName);
        Assert.Equal(walletOwner, result.Owner);
        Assert.Equal(WalletType.MOMO, result.WalletType);
        Assert.Equal(AccountScheme.VODAFONE, result.AccountScheme);
        Assert.False(result.IsActive);
        Assert.NotNull(result.UpdatedAt);
    }
}
