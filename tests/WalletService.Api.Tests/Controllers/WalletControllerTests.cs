using AutoMapper;
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
    #region AddWallet

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
    #endregion

    #region GetWallet
    [Fact]
    public async void GetWallet_WalletExists_Success()
    {
        // Arrange
        var walletId = 1;
        var wallet = new Wallet("Frodo Baggins", "368790", InternalWalletType.CARD, InternalAccountScheme.MASTERCARD, "1234567890");


        var repositoryMock = new Mock<IWalletRepository>();
        repositoryMock.Setup(x => x.GetWalletById(walletId)).ReturnsAsync(wallet).Verifiable(Times.Once);

        var loggerMock = new Mock<ILogger<WalletController>>();

        var dto = new WalletReadDto
        {
            Id = walletId,
            AccountNumber = wallet.AccountNumber,
            WalletName = wallet.WalletName,
            AccountScheme = (AccountScheme)wallet.AccountScheme,
            WalletType = (WalletType)wallet.WalletType,
            OwnerPhoneNumber = wallet.Owner
        };
        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(x => x.Map<WalletReadDto>(It.IsAny<Wallet>())).Returns(dto).Verifiable(Times.Once);

        var controller = CreateWalletController(walletRepositoryMock: repositoryMock.Object, 
            logger: loggerMock.Object, mapperMock: mapperMock.Object);


        // Act 
        var response = await controller.GetWalletById(walletId) as OkObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        var retrievedWallet = response.Value as WalletReadDto;
        Assert.NotNull(retrievedWallet);
        Assert.Equal(wallet.WalletName, retrievedWallet.WalletName);
        Assert.Equal(wallet.AccountNumber, retrievedWallet.AccountNumber);
        Assert.Equal(wallet.Owner, retrievedWallet.OwnerPhoneNumber);
        Assert.Equal((WalletType)wallet.WalletType, retrievedWallet.WalletType);
        Assert.Equal((AccountScheme)wallet.AccountScheme, retrievedWallet.AccountScheme);

        mapperMock.Verify();

        repositoryMock.Verify();
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void GetWallet_WalletDoesNotExists_ReturnsNotFound()
    {
        // Arrange
        var walletId = 1;

        var repositoryMock = new Mock<IWalletRepository>();
        repositoryMock.Setup(x => x.GetWalletById(walletId)).ReturnsAsync((Wallet)null!).
            Verifiable(Times.Once);

        var loggerMock = new Mock<ILogger<WalletController>>();

        var mapperMock = new Mock<IMapper>();
        var controller = CreateWalletController(walletRepositoryMock: repositoryMock.Object,
            logger: loggerMock.Object, mapperMock: mapperMock.Object);

        // Act 
        var response = await controller.GetWalletById(walletId) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);

        mapperMock.VerifyNoOtherCalls();

        repositoryMock.Verify();
        repositoryMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async void GetWallet_InvalidWalletId_ThrowsArgumentOutOfRangeException(int walletId)
    {
        // Arrange
        var repositoryMock = new Mock<IWalletRepository>();
        var loggerMock = new Mock<ILogger<WalletController>>();
        var mapperMock = new Mock<IMapper>();
        var controller = CreateWalletController(walletRepositoryMock: repositoryMock.Object,
            logger: loggerMock.Object, mapperMock: mapperMock.Object);

        // Act 
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async () => await controller.GetWalletById(walletId));

        // Assert
        mapperMock.VerifyNoOtherCalls();

        repositoryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region GetWallet
    [Fact]
    public async void GetWallets_RepositoryReturnsListOfWallets_Success()
    {
        // Arrange
        Wallet wallet1 = new("Bilbo Baggins", "368790", InternalWalletType.CARD, InternalAccountScheme.MASTERCARD, "1234567890"); 
        Wallet wallet2 = new("Frodo's Voda", "0244123456", InternalWalletType.MOMO, InternalAccountScheme.VODAFONE, "0204123456");

        var repositoryMock = new Mock<IWalletRepository>();
        repositoryMock.Setup(x => x.GetAllWallets()).ReturnsAsync([wallet1, wallet2]).Verifiable(Times.Once);

        var loggerMock = new Mock<ILogger<WalletController>>();

        var expectedResponseData = new List<WalletReadDto> {
        new() {
            Id = wallet1.Id,
            AccountNumber = wallet1.AccountNumber,
            WalletName = wallet1.WalletName,
            AccountScheme = (AccountScheme)wallet1.AccountScheme,
            WalletType = (WalletType)wallet1.WalletType,
            OwnerPhoneNumber = wallet1.Owner
        },
        new() {
            Id = wallet2.Id,
            AccountNumber = wallet2.AccountNumber,
            WalletName = wallet2.WalletName,
            AccountScheme = (AccountScheme)wallet2.AccountScheme,
            WalletType = (WalletType)wallet2.WalletType,
            OwnerPhoneNumber = wallet2.Owner
        }
        };
        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(x => x.Map<IEnumerable<WalletReadDto>>(It.IsAny<IEnumerable<Wallet>>())).Returns(expectedResponseData).Verifiable(Times.Once);

        var controller = CreateWalletController(walletRepositoryMock: repositoryMock.Object,
            logger: loggerMock.Object, mapperMock: mapperMock.Object);


        // Act 
        var response = await controller.GetAllWallets() as OkObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);

        var retrievedWallets = response.Value as List<WalletReadDto>;
        Assert.NotNull(retrievedWallets);
        Assert.NotEmpty(retrievedWallets);
        Assert.Equivalent(expectedResponseData, retrievedWallets);

        mapperMock.Verify();

        repositoryMock.Verify();
        repositoryMock.VerifyNoOtherCalls();
    }
    [Fact]
    public async void GetWallets_RepositoryReturnsEmptyListOfWallets_Success()
    {
        // Arrange
        var repositoryMock = new Mock<IWalletRepository>();
        repositoryMock.Setup(x => x.GetAllWallets()).ReturnsAsync(Enumerable.Empty<Wallet>).Verifiable(Times.Once);

        var loggerMock = new Mock<ILogger<WalletController>>();

        var mapperMock = new Mock<IMapper>();

        var controller = CreateWalletController(walletRepositoryMock: repositoryMock.Object,
            logger: loggerMock.Object, mapperMock: mapperMock.Object);

        // Act 
        var response = await controller.GetAllWallets() as OkObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);

        var retrievedWallets = response.Value as List<WalletReadDto>;

        Assert.NotNull(retrievedWallets);
        Assert.Empty(retrievedWallets);

        repositoryMock.Verify();
        repositoryMock.VerifyNoOtherCalls();

        mapperMock.VerifyNoOtherCalls();
    }

    #endregion

    #region DeleteWallet
    [Fact]
    public async void DeleteWallet_WalletExists_Success()
    {
        // Arrange
        var walletId = 1;
        var wallet = new Wallet("Frodo Baggins", "368790", InternalWalletType.CARD, InternalAccountScheme.MASTERCARD, "1234567890");

        
        var repositoryMock = new Mock<IWalletRepository>();
        repositoryMock.Setup(x => x.MarkAsDeleted(walletId)).ReturnsAsync(wallet).Verifiable(Times.Once);

        var loggerMock = new Mock<ILogger<WalletController>>();

        var dto = new WalletReadDto
        {
            Id = walletId,
            AccountNumber = wallet.AccountNumber,
            WalletName = wallet.WalletName,
            AccountScheme = (AccountScheme)wallet.AccountScheme,
            WalletType = (WalletType)wallet.WalletType,
            OwnerPhoneNumber = wallet.Owner
        };
        var mapperMock = new Mock<IMapper>();
        mapperMock.Setup(x => x.Map<WalletReadDto>(It.IsAny<Wallet>())).Returns(dto).Verifiable(Times.Once);

        var controller = CreateWalletController(walletRepositoryMock: repositoryMock.Object, logger: loggerMock.Object, mapperMock: mapperMock.Object);


        // Act 
        var response = await controller.DeleteWallet(walletId) as OkObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status200OK, response.StatusCode);
        var deletedWallet = response.Value as WalletReadDto;
        Assert.NotNull(deletedWallet);
        Assert.Equal(wallet.WalletName, deletedWallet.WalletName);
        Assert.Equal(wallet.AccountNumber, deletedWallet.AccountNumber);
        Assert.Equal(wallet.Owner, deletedWallet.OwnerPhoneNumber);
        Assert.Equal((WalletType)wallet.WalletType, deletedWallet.WalletType);
        Assert.Equal((AccountScheme)wallet.AccountScheme, deletedWallet.AccountScheme);

        mapperMock.Verify();

        repositoryMock.Verify();
        repositoryMock.VerifyNoOtherCalls();
    }

    [Fact]
    public async void DeleteWallet_WalletDoesNotExists_ReturnsNotFound()
    {
        // Arrange
        var walletId = 1;

        var repositoryMock = new Mock<IWalletRepository>();
        repositoryMock.Setup(x => x.MarkAsDeleted(walletId)).ReturnsAsync((Wallet)null!).
            Verifiable(Times.Once);

        var loggerMock = new Mock<ILogger<WalletController>>();

        var mapperMock = new Mock<IMapper>();
        var controller = CreateWalletController(walletRepositoryMock: repositoryMock.Object, 
            logger: loggerMock.Object, mapperMock: mapperMock.Object);

        // Act 
        var response = await controller.DeleteWallet(walletId) as NotFoundObjectResult;

        // Assert
        Assert.NotNull(response);
        Assert.Equal(StatusCodes.Status404NotFound, response.StatusCode);

        mapperMock.VerifyNoOtherCalls();

        repositoryMock.Verify();
        repositoryMock.VerifyNoOtherCalls();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    public async void DeleteWallet_InvalidWalletId_ThrowsArgumentOutOfRangeException(int walletId)
    {
        // Arrange

        var repositoryMock = new Mock<IWalletRepository>();
        var loggerMock = new Mock<ILogger<WalletController>>();
        var mapperMock = new Mock<IMapper>();
        var controller = CreateWalletController(walletRepositoryMock: repositoryMock.Object, 
            logger: loggerMock.Object, mapperMock: mapperMock.Object);

        // Act 
        await Assert.ThrowsAsync<ArgumentOutOfRangeException>(async ()=>  await controller.DeleteWallet(walletId));

        // Assert
        mapperMock.VerifyNoOtherCalls();

        repositoryMock.VerifyNoOtherCalls();
    }
    #endregion

    #region Helpers

    private static WalletController CreateWalletController(IWalletRepository walletRepositoryMock, IWalletValidator? walletValidator = null, ILogger<WalletController>? logger = null, IMapper? mapperMock = null)
    {
        mapperMock = mapperMock ?? new Mock<IMapper>().Object;
        walletValidator ??= new Mock<IWalletValidator>().Object;
        logger ??= new Mock<ILogger<WalletController>>().Object;
        return new WalletController(walletRepositoryMock, walletValidator, logger, mapperMock);
    }
    #endregion
}
