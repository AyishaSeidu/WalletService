﻿using Microsoft.EntityFrameworkCore;
using MockQueryable.Moq;
using Moq;
using WalletService.Domain.Models;
using WalletService.Domain.Models.Enums;
using WalletService.Infrastructure.DataContext;
using WalletService.Infrastructure.Repository;

namespace WalletService.Infrastructure.Tests.Repositories;

public class WalletRespositoryTests
{
    #region AddWallet
    [Fact]
    public async Task AddWallet_WalletOwnerLessThan5ActiveWallets_Success()
    {
        // Arrage
        var existingWallets = new List<Wallet>()
        {
            new("Frodo's Mtn", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.MTN, "0244123456"),
            new("Frodo's Voda", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.VODAFONE, "0204123456"),
            new("Frodo's Mastercard", "123456", InternalWalletType.CARD, InternalAccountScheme.MASTERCARD, "0244123456"),
            new("Bilbo's Mastercard", "567890", InternalWalletType.CARD, InternalAccountScheme.MASTERCARD, "0240123789")
        }.AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<IWalletServiceContext>();
        contextMock.Setup(x => x.Wallets).Returns(existingWallets.Object).Verifiable(Times.Exactly(3));
        contextMock.Setup(x => x.Wallets.Add(It.IsAny<Wallet>())).Verifiable(Times.Once());
        contextMock.Setup(x=> x.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable(Times.Once());

        var repository = CreateWalletRepository(contextMock);

        var newWallet = new Wallet("Frodo's Visa", "192837", InternalWalletType.CARD, InternalAccountScheme.VISA, "0244123456");
        // Act
        var result = await repository.AddWallet(newWallet);

        // Assert
        contextMock.Verify();
    }

    [Fact]
    public async Task AddWallet_WalletOwnerHasMoreThan5ActiveWallets_ThrowsInvalidOperationException()
    {
        // Arrage
        var existingWallets = new List<Wallet>()
        {
            new("Frodo's Mtn", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.MTN, "0244123456"),
            new("Frodo's Voda", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.VODAFONE, "0244123456"),
            new("Frodo's Mastercard", "123456", InternalWalletType.CARD, InternalAccountScheme.MASTERCARD, "0244123456"),
            new("Frodo's AirtelTigo", "0266123456", InternalWalletType.MOMO, InternalAccountScheme.AIRTELTIGO, "0244123456"),
            new("Frodo's other AirtelTigo", "0267123456", InternalWalletType.MOMO, InternalAccountScheme.AIRTELTIGO, "0244123456")
        }.AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<IWalletServiceContext>();
        contextMock.Setup(x => x.Wallets).Returns(existingWallets.Object).Verifiable(Times.Exactly(2));

        var repository = CreateWalletRepository(contextMock);

        var newWallet = new Wallet("Frodo's Visa", "192837", InternalWalletType.CARD, InternalAccountScheme.VISA, "0244123456");

        /// Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.AddWallet(newWallet));
        contextMock.Verify();
    }

    [Fact]
    public async Task AddWallet_WalletOwnerHasMoreThan5WalletsButInactive_ThrowsInvalidOperationException()
    {
        // Arrage
        var wallets = new List<Wallet>()
        {
            new("Frodo's Mtn", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.MTN, "0244123456"),
            new("Frodo's Voda", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.VODAFONE, "0244123456"),
            new("Frodo's Mastercard", "123456", InternalWalletType.CARD, InternalAccountScheme.MASTERCARD, "0244123456"),
            new("Frodo's AirtelTigo", "0266123456", InternalWalletType.MOMO, InternalAccountScheme.AIRTELTIGO, "0244123456"),
            new("Frodo's other AirtelTigo", "0267123456", InternalWalletType.MOMO, InternalAccountScheme.AIRTELTIGO, "0244123456"),
            new("Frodo's Stanbic Visa", "654321", InternalWalletType.CARD, InternalAccountScheme.VISA, "0244123456")
        };

        wallets.ForEach(x => x.DeactivateWallet());

        var existingWallets = wallets.AsQueryable().BuildMockDbSet();
        var contextMock = new Mock<IWalletServiceContext>();
        contextMock.Setup(x => x.Wallets).Returns(existingWallets.Object).Verifiable(Times.Exactly(3));
        contextMock.Setup(x => x.Wallets.Add(It.IsAny<Wallet>())).Verifiable(Times.Once());
        contextMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable(Times.Once());

        var repository = CreateWalletRepository(contextMock);

        var newWallet = new Wallet("Frodo's Visa", "192837", InternalWalletType.CARD, InternalAccountScheme.VISA, "0244123456");

        // Act
        var result = await repository.AddWallet(newWallet);

        // Assert
        contextMock.Verify();
    }


    [Fact]
    public async Task AddWallet_WalletAlreadyExistsForTheSameOwner_ThrowsInvalidOperationException()
    {
        // Arrage
        var existingWallets = new List<Wallet>()
        {
            new("Frodo's Mtn", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.MTN, "0244123456")
        }.AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<IWalletServiceContext>();
        contextMock.Setup(x => x.Wallets).Returns(existingWallets.Object).Verifiable(Times.Once);

        var repository = CreateWalletRepository(contextMock);

        var newWallet = new Wallet("MTN", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.AIRTELTIGO, "0244123456");

        // Act & Assert
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await repository.AddWallet(newWallet));

        contextMock.Verify();
    }

    [Fact]
    public async Task AddWallet_WalletWithSameAccountExistsForAnotherOwner_Sucess()
    {
        // Arrage
        var existingWallets = new List<Wallet>()
        {
            new("Frodo's Mtn", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.MTN, "0244123456")
        }.AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<IWalletServiceContext>();
        contextMock.Setup(x => x.Wallets).Returns(existingWallets.Object).Verifiable(Times.Exactly(3));
        contextMock.Setup(x => x.Wallets.Add(It.IsAny<Wallet>())).Verifiable(Times.Once());
        contextMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable(Times.Once());

        var repository = CreateWalletRepository(contextMock);

        var newWallet = new Wallet("Gandalf Ventures MTN", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.AIRTELTIGO, "0302876543");

        // Act
        var result = await repository.AddWallet(newWallet);

        // Assert
        contextMock.Verify();
    }

    [Fact]
    public async Task AddWallet_DbUpdateThrowsException_ThrowsSameException()
    {
        // Arrage
        var existingWallets = new List<Wallet>()
        {
            new("Frodo's Mtn", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.MTN, "0244123456"),
            new("Frodo's Voda", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.VODAFONE, "0204123456"),
            new("Frodo's Mastercard", "123456", InternalWalletType.CARD, InternalAccountScheme.MASTERCARD, "0244123456"),
            new("Bilbo's Mastercard", "567890", InternalWalletType.CARD, InternalAccountScheme.MASTERCARD, "0240123789")
        }.AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<IWalletServiceContext>();
        contextMock.Setup(x => x.Wallets).Returns(existingWallets.Object).Verifiable(Times.Exactly(3));
        contextMock.Setup(x => x.Wallets.Add(It.IsAny<Wallet>())).Verifiable(Times.Once());
        contextMock.Setup(x => x.SaveChangesAsync()).ThrowsAsync(new DbUpdateException()).Verifiable(Times.Once());

        var repository = CreateWalletRepository(contextMock);

        var newWallet = new Wallet("Frodo's Visa", "192837", InternalWalletType.CARD, InternalAccountScheme.VISA, "0244123456");

        // Act & Assert
        await Assert.ThrowsAsync<DbUpdateException>(async () => await repository.AddWallet(newWallet));

        contextMock.Verify();
    }

    [Fact]
    public async Task AddWallet_NullInputAsWallet_ThrowsArgumentException()
    {
        // Arrage
        var repository = CreateWalletRepository();

        var newWallet = (Wallet)null!;

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentNullException>(async () => await repository.AddWallet(newWallet));
    }
    #endregion

    #region MarkWalletAsDeleted
    [Fact]
    public async Task MarkAsDeleted_WalletExitsAndIsActive_Success()
    {
        // Arrange
        var wallet = new Wallet("Frodo's Mtn", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.MTN, "0244123456");
        var existingWallets = new List<Wallet>()
        {
            wallet
        }.AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<IWalletServiceContext>();
        contextMock.Setup(x => x.Wallets).Returns(existingWallets.Object).Verifiable(Times.Exactly(2));
        contextMock.Setup(x => x.Wallets.Update(It.IsAny<Wallet>())).Verifiable(Times.Once());
        contextMock.Setup(x => x.SaveChangesAsync()).Returns(Task.CompletedTask).Verifiable(Times.Once());

        var repository = CreateWalletRepository(contextMock);

        // Act
        var result = await repository.MarkAsDeleted(wallet.Id);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(wallet.Id, result.Id);
        Assert.Equal(wallet.WalletName, result.WalletName);
        Assert.Equal(wallet.AccountNumber, result.AccountNumber);
        Assert.Equal(wallet.WalletType, result.WalletType);
        Assert.Equal(wallet.AccountScheme, result.AccountScheme);
        Assert.Equal(wallet.Owner, result.Owner);
        Assert.False(result.IsActive);
        Assert.NotNull(result.UpdatedAt);

        contextMock.Verify();
    }

    [Fact]
    public async Task MarkAsDeleted_WalletDoesNotExits_ReturnsNull()
    {
        // Arrange
        var wallet = new Wallet("Frodo's Mtn", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.MTN, "0244123456");
        int aNonExistingWalletId = 100;
        var existingWallets = new List<Wallet>()
        {
            wallet
        }.AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<IWalletServiceContext>();
        contextMock.Setup(x => x.Wallets).Returns(existingWallets.Object).Verifiable(Times.Once);

        var repository = CreateWalletRepository(contextMock);

        // Act
        var result = await repository.MarkAsDeleted(aNonExistingWalletId);

        // Assert
        Assert.Null(result);
        contextMock.Verify();
    }
    [Fact]
    public async Task MarkAsDeleted_WalletExitsAndIsInactive_ReturnsNull()
    {
        // Arrange
        var wallet = new Wallet("Frodo's Mtn", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.MTN, "0244123456");
        wallet.DeactivateWallet();
        var existingWallets = new List<Wallet>()
        {
            wallet
        }.AsQueryable().BuildMockDbSet();

        var contextMock = new Mock<IWalletServiceContext>();
        contextMock.Setup(x => x.Wallets).Returns(existingWallets.Object).Verifiable(Times.Once);
      
        var repository = CreateWalletRepository(contextMock);

        // Act
        var result = await repository.MarkAsDeleted(wallet.Id);

        // Assert
        Assert.Null(result);

        contextMock.Verify();
    }
    #endregion
    #region Hepers

    private WalletRepository CreateWalletRepository(Mock<IWalletServiceContext>? contextMock = null)
    {
        contextMock ??= new Mock<IWalletServiceContext>();
        return new WalletRepository(contextMock.Object);
    }

    #endregion
}
