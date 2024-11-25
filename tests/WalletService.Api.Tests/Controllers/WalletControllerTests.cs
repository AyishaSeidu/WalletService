using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WalletService.Api.Application.Dtos;
using WalletService.Api.Application.Dtos.Enums;
using WalletService.Api.Application.Validation;
using WalletService.Api.Controllers;
using WalletService.Domain.Models;
using WalletService.Domain.Models.Enums;
using WalletService.Infrastructure.Repository.Interfaces;

namespace WalletService.Api.Tests.Controllers;

public class WalletControllerTests
{

    [Fact]
    public async void AddWallet_ValidRequest_Success()
    {
        // Arrange
        var walletId = 1;
        var request = new WalletWriteDto
        {
            AccountNumber = "0243567890",
            WalletName = "Frodo Baggins",
            WalletType = WalletType.MOMO,
            AccountScheme = AccountScheme.MTN,
            OwnerPhoneNumber = "1234567890"
        };

        var validatorMock = new Mock<IWalletValidator>();
        validatorMock.Setup(x=> x.ValidateWallet(request)).Returns(new ValidationResult(true)).Verifiable(Times.Once);

        var repositoryMock = new Mock<IWalletRepository>();
        repositoryMock.Setup(x=> x.AddWallet(It.Is<Wallet>(w=> w.AccountNumber == request.AccountNumber && w.WalletName == request.WalletName && w.WalletType==InternalWalletType.MOMO && w.AccountScheme == InternalAccountScheme.MTN))).ReturnsAsync(walletId).Verifiable(Times.Once);

        var loggerMock = new Mock<ILogger<WalletController>>();
        var controller = CreateWalletController(repositoryMock.Object, validatorMock.Object, loggerMock.Object);
        
        // Act 
        var response = await controller.AddWallet(request) as OkObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        Assert.Equal(walletId, response.Value);

        validatorMock.Verify();
        validatorMock.VerifyNoOtherCalls();

        repositoryMock.Verify();
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void AddWallet_ValidatorReturnsInvalidStatus_ThrowsArgumentException()
    {
        // Arrange
        var walletId = 1;
        var request = new WalletWriteDto
        {
            AccountNumber = "0243567890",
            WalletName = "Frodo Baggins",
            WalletType = WalletType.MOMO,
            AccountScheme = AccountScheme.MASTERCARD,
            OwnerPhoneNumber = "1234567890"
        };

        var validatorMock = new Mock<IWalletValidator>();
        validatorMock.Setup(x => x.ValidateWallet(request)).Returns(new ValidationResult(false, InvalidReasons.InvalidAccountSchemeForMoMoWallets)).Verifiable(Times.Once);

        var repositoryMock = new Mock<IWalletRepository>();

        var loggerMock = new Mock<ILogger<WalletController>>();
        var controller = CreateWalletController(repositoryMock.Object, validatorMock.Object, loggerMock.Object);

        // Act 
        await Assert.ThrowsAsync<ArgumentException>(async () => await controller.AddWallet(request));

        // Assert
        validatorMock.Verify();
        validatorMock.VerifyNoOtherCalls();

        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void AddWallet_RepositoryThrowsException_ThrowsSameException()
    {
        // Arrange
        var walletId = 1;
        var request = new WalletWriteDto
        {
            AccountNumber = "0243567890",
            WalletName = "Frodo Baggins",
            WalletType = WalletType.MOMO,
            AccountScheme = AccountScheme.MTN,
            OwnerPhoneNumber = "1234567890"
        };

        var validatorMock = new Mock<IWalletValidator>();
        validatorMock.Setup(x => x.ValidateWallet(request)).Returns(new ValidationResult(true)).Verifiable(Times.Once);

        var repositoryMock = new Mock<IWalletRepository>();
        repositoryMock.Setup(x => x.AddWallet(It.Is<Wallet>(w => w.AccountNumber == request.AccountNumber && w.WalletName == request.WalletName && w.WalletType == InternalWalletType.MOMO && w.AccountScheme == InternalAccountScheme.MTN))).ThrowsAsync(new InvalidOperationException()).Verifiable(Times.Once);

        var loggerMock = new Mock<ILogger<WalletController>>();
        var controller = CreateWalletController(repositoryMock.Object, validatorMock.Object, loggerMock.Object);

        // Act 
        await Assert.ThrowsAsync<InvalidOperationException>(async () => await controller.AddWallet(request));

        // Assert
        validatorMock.Verify();
        validatorMock.VerifyNoOtherCalls();

        repositoryMock.Verify();
        repositoryMock.VerifyNoOtherCalls();
    }

    #region
    private static WalletController CreateWalletController(IWalletRepository walletRepositoryMock, IWalletValidator walletValidator, ILogger<WalletController> logger)
    {
        return new WalletController(walletRepositoryMock, walletValidator, logger);
    }
    #endregion
}
