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
        logger.LogInformation($"WalletService: Start AddWallet-> Adding a new wallet for user {wallet.OwnerPhoneNumber}");
        var validationResult = walletValidator.ValidateWallet(wallet);
        if (validationResult.IsValid == false)
        {
            logger.LogDebug($"WalletService: Stop AddWallet-> Invalid request : {validationResult.InvalidReason}");
            throw new ArgumentException(validationResult.InvalidReason);
        }
        var walletToAdd = new Wallet(wallet.WalletName, wallet.AccountNumber, GetInternalWalletType(wallet.WalletType), GetInternalAccountScheme(wallet.AccountScheme), wallet.OwnerPhoneNumber);
        logger.LogInformation($"WalletService:  AddWallet -> New wallet validated, saving wallet for user {walletToAdd.Owner}");

        var walletAdded = await walletRepository.AddWallet(walletToAdd);

        logger.LogInformation($"WalletService: End AddWallet -> New wallet: {walletAdded.Id} added for user {walletAdded.Owner}");
        return CreatedAtAction(nameof(GetWalletById), new { id = walletAdded.Id }, mapper.Map<WalletReadDto>(walletAdded));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetWalletById(int id)
    {
        logger.LogInformation($"WalletService: Start GetWalletById -> Retrieving wallet: {id}");

        ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);
        var wallet = await walletRepository.GetWalletById(id);

        if (wallet is null)
        {
            logger.LogInformation($"WalletService: End GetWalletById -> Wallet Id: {id}, not found");
            return NotFound($"Wallet with id: {id} not found");
        }
        logger.LogInformation($"WalletService: End GetWalletById -> Retrieved wallet: {id}");

        return Ok(mapper.Map<WalletReadDto>(wallet));
    }

    [HttpGet("user/{phoneNumber}")]
    public async Task<IActionResult> GetWalletsByUserId(string phoneNumber)
    {
        logger.LogInformation($"WalletService: Start GetWalletByUserId -> Retrieving wallets for user: {phoneNumber}");

        ArgumentException.ThrowIfNullOrWhiteSpace(phoneNumber);
        var wallets = await walletRepository.GetWalletsByUserId(phoneNumber);

        if (!wallets.Any())
        {
            logger.LogInformation($"WalletService: End GetWalletsByUserId -> No wallets found for user {phoneNumber}: returning an empty list");
            return Ok(new List<WalletReadDto>());
        }

        logger.LogInformation($"WalletService: GetWalletsByUserId -> Found {wallets.Count()} wallets for user {phoneNumber}");
        logger.LogInformation($"WalletService: End GetWalletsByUserId -> Retrieving wallets");

        return Ok(mapper.Map<List<WalletReadDto>>(wallets));
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
        logger.LogInformation($"WalletService: Start DeleteWallet -> Deleting wallet: {id}");

        ArgumentOutOfRangeException.ThrowIfLessThan(id, 1);
        var deletedWallet = await walletRepository.MarkAsDeleted(id);

        if (deletedWallet is null)
        {
            logger.LogInformation($"WalletService: End DeleteWallet -> Wallet Id: {id}, wallet not found");
            return NotFound($"Wallet with id: {id} not found");
        }
        logger.LogInformation($"WalletService: End DeleteWallet -> Deleted wallet: {id}");
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