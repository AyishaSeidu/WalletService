using System.Text.RegularExpressions;
using WalletService.Api.Application.Dtos;
using WalletService.Api.Application.Dtos.Enums;

namespace WalletService.Api.Application.Validation;

public class WalletValidator : IWalletValidator
{
    public IValidationResult ValidateWallet(WalletWriteDto wallet)
    {
        if (wallet == null)
        {
            return new ValidationResult(false, InvalidReasons.NullWallet);
        }
        if (string.IsNullOrWhiteSpace(wallet.WalletName))
        {
            return new ValidationResult(false, InvalidReasons.InvalidWalletName);
        }
        if (string.IsNullOrWhiteSpace(wallet.OwnerPhoneNumber))
        {
            return new ValidationResult(false, InvalidReasons.EmptyWalletOwnerNumber);
        }
        var internationalNumberPattern = new Regex(@"^\d{7,15}$");
        if (!internationalNumberPattern.IsMatch(wallet.OwnerPhoneNumber))
        {
            return new ValidationResult(false, InvalidReasons.InvalidWalletOwnerPhoneNumberFormat);
        }
        if (wallet.WalletType == WalletType.UNKNOWN)
        {
            return new ValidationResult(false, InvalidReasons.UnknownWalletType);
        }
        if (wallet.AccountScheme == AccountScheme.UNKNOWN)
        {
            return new ValidationResult(false, InvalidReasons.UnknownAccountScheme);
        }

        if (string.IsNullOrWhiteSpace(wallet.AccountNumber))
        {
            return new ValidationResult(false, InvalidReasons.EmptyAccountNumber);
        }
        var localNumberPattern = new Regex(@"^0\d{9}$");
        if (wallet.WalletType == WalletType.MOMO && !localNumberPattern.IsMatch(wallet.AccountNumber))
        {
            return new ValidationResult(false, InvalidReasons.InvalidMoMoNumber);
        }
        var cardAccountNummberPattern = new Regex("^\\d{6}$");
        if (wallet.WalletType == WalletType.CARD && !cardAccountNummberPattern.IsMatch(wallet.AccountNumber))
        {
            return new ValidationResult(false, InvalidReasons.InvalidCardNumber);
        }

        if (wallet.WalletType == WalletType.MOMO && (wallet.AccountScheme != AccountScheme.AIRTELTIGO && wallet.AccountScheme != AccountScheme.MTN && wallet.AccountScheme != AccountScheme.VODAFONE))
        {
            return new ValidationResult(false, InvalidReasons.InvalidAccountSchemeForMoMoWallets);
        }
        if (wallet.WalletType == WalletType.CARD && (wallet.AccountScheme != AccountScheme.MASTERCARD && wallet.AccountScheme != AccountScheme.VISA))
        {
            return new ValidationResult(false, InvalidReasons.InvalidAccountSchemeForCardWallets);
        }


        return new ValidationResult(true);

    }
}
