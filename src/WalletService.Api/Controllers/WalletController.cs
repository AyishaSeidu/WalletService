using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using WalletService.Api.Application.Dtos;
using WalletService.Api.Application.Dtos.Enums;
using WalletService.Api.Application.Validation;
using WalletService.Api.Controllers.Filters;
using WalletService.Domain.Models;
using WalletService.Domain.Models.Enums;
using WalletService.Infrastructure.Repository.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace WalletService.Api.Controllers;

[Route("api/wallets")]
[ApiController]
[ServiceFilter(typeof(ExceptionFilter<WalletController>))]

public class WalletController(IWalletRepository walletRepository, IWalletValidator walletValidator, ILogger<WalletController> logger, IMapper mapper) : ControllerBase
{

    [HttpPost]
    public async Task<IActionResult> AddWallet([FromBody] WalletWriteDto wallet)
    {
        logger.LogInformation($"Wallet Service: Start -> Adding a new wallet for user {wallet.OwnerPhoneNumber}");
        var validationResult = walletValidator.ValidateWallet(wallet);
        if (validationResult.IsValid == false)
        {
            logger.LogDebug($"Wallet Service: Stop -> Adding wallet: Invalid request : {validationResult.InvalidReason}");
            throw new ArgumentException(validationResult.InvalidReason);
        }
        var walletToAdd = new Wallet(wallet.WalletName, wallet.AccountNumber, GetInternalWalletType(wallet.WalletType), GetInternalAccountScheme(wallet.AccountScheme), wallet.OwnerPhoneNumber);
        logger.LogInformation($"WalletService: -> New wallet validated, saving wallet for user {walletToAdd.Owner}");

        var walletAdded = await walletRepository.AddWallet(walletToAdd);

        logger.LogInformation($"Wallet Service: End -> New wallet: {walletAdded.Id} added for user {walletAdded.Owner}");
        return CreatedAtAction(nameof(GetWalletById), new { id = walletAdded.Id }, mapper.Map<WalletReadDto>(walletAdded));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWalletById(int id)
    {
        logger.LogInformation($"WalletService: Start -> Retrieving wallet: {id}");

        ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);
        var wallet = await walletRepository.GetWalletById(id);

        if (wallet is null)
        {
            logger.LogInformation($"WalletService: End -> Retrieving wallet: {id}, wallet not found");
            return NotFound($"Wallet with id: {id} not found");
        }
        logger.LogInformation($"WalletService: End -> Retrieving wallet: {id}");

        return Ok(mapper.Map<WalletReadDto>(wallet));
    }

    [HttpGet]
    public async Task<IActionResult> GetAllWallets()
    {
        logger.LogInformation($"WalletService: Start GetAllWallets -> Retrieving all wallets");

        var wallets = await walletRepository.GetAllWallets();

        if (!wallets.Any())
        {
            logger.LogInformation($"WalletService:  End GetAllWallets -> No wallets found: returning an empty list");
            return Ok(new List<WalletReadDto>());
        }

        logger.LogInformation($"WalletService: GetAllWallets -> Found {wallets.Count()} wallets ");
        logger.LogInformation($"WalletService: End GetAllWallets -> Retrieving wallets");
        return Ok(mapper.Map<List<WalletReadDto>>(wallets));
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteWallet(int id)
    {
        logger.LogInformation($"WalletService: Start -> Deleting wallet: {id}");

        ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);
        var deletedWallet = await walletRepository.MarkAsDeleted(id);

        if (deletedWallet is null)
        {
            logger.LogInformation($"WalletService: End -> Deleting wallet: {id}, wallet not found");
            return NotFound($"Wallet with id: {id} not found");
        }
        logger.LogInformation($"WalletService: End -> Deleting wallet: {id}");
        return Ok(mapper.Map<WalletReadDto>(deletedWallet));
    }
    private static InternalWalletType GetInternalWalletType(WalletType externalWalletType)
    {
        return externalWalletType switch
        {
            WalletType.UNKNOWN => throw new ArgumentException("Unsurpported wallet type"),
            WalletType.MOMO => InternalWalletType.MOMO,
            WalletType.CARD => InternalWalletType.CARD,
            _ => throw new ArgumentException("Unsurpported wallet type"),
        };
    }

    private static InternalAccountScheme GetInternalAccountScheme(AccountScheme externalAccountScheme)
    {
        return externalAccountScheme switch
        {
            AccountScheme.UNKNOWN => throw new ArgumentException("Unsurpported account scheme"),
            AccountScheme.AIRTELTIGO => InternalAccountScheme.AIRTELTIGO,
            AccountScheme.MTN => InternalAccountScheme.MTN,
            AccountScheme.VODAFONE => InternalAccountScheme.VODAFONE,
            AccountScheme.MASTERCARD => InternalAccountScheme.MASTERCARD,
            AccountScheme.VISA => InternalAccountScheme.VISA,
            _ => throw new ArgumentException("Unsurpported wallet type"),
        };
    }
}