using WalletService.Api.Application.Dtos;
using WalletService.Api.Application.Dtos.Enums;
using WalletService.Api.Application.Validation;

namespace WalletService.Api.Tests.Application.Validation;

public class WalletValidatorTests
{
    [Theory]
    [MemberData(nameof(WalletDetailsWithExpectedValidationDetails))]
    public void ValidateWallet_ValidatesCorrectly(WalletWriteDto wallet, ValidationResult expectedValidationResult)
    {
        // Arrange
        var validator = new WalletValidator();

        // Act
        var actualValidationResult = validator.ValidateWallet(wallet);

        Assert.NotNull(actualValidationResult);
        Assert.Equal(expectedValidationResult.IsValid, actualValidationResult.IsValid);
        Assert.Equal(expectedValidationResult.InvalidReason, actualValidationResult.InvalidReason);
    }

    public static TheoryData<WalletWriteDto, ValidationResult> WalletDetailsWithExpectedValidationDetails => new()
    {
        {
            (WalletWriteDto)null!,
            new ValidationResult(false, InvalidReasons.NullWallet)
        },
        {
            new WalletWriteDto
            {
                WalletName = "",
                AccountNumber = "0244000000",
                WalletType = WalletType.MOMO,
                AccountScheme = AccountScheme.MTN,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(false, InvalidReasons.InvalidWalletName)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "0244000000",
                WalletType = WalletType.MOMO,
                AccountScheme = AccountScheme.MTN,
                OwnerPhoneNumber = ""
            },
            new ValidationResult(false, InvalidReasons.EmptyWalletOwnerNumber)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "0244000000",
                WalletType = WalletType.MOMO,
                AccountScheme = AccountScheme.MTN,
                OwnerPhoneNumber = "123456"
            },
            new ValidationResult(false, InvalidReasons.InvalidWalletOwnerPhoneNumberFormat)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "0244000000",
                WalletType = WalletType.UNKNOWN,
                AccountScheme = AccountScheme.VISA,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(false, InvalidReasons.UnknownWalletType)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "0244000000",
                WalletType = WalletType.MOMO,
                AccountScheme = AccountScheme.UNKNOWN,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(false, InvalidReasons.UnknownAccountScheme)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "",
                WalletType = WalletType.MOMO,
                AccountScheme = AccountScheme.VISA,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(false, InvalidReasons.EmptyAccountNumber)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "244000000",
                WalletType = WalletType.MOMO,
                AccountScheme = AccountScheme.MASTERCARD,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(false, InvalidReasons.InvalidMoMoNumber)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "233244000000",
                WalletType = WalletType.MOMO,
                AccountScheme = AccountScheme.MASTERCARD,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(false, InvalidReasons.InvalidMoMoNumber)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "0244000000",
                WalletType = WalletType.CARD,
                AccountScheme = AccountScheme.MASTERCARD,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(false, InvalidReasons.InvalidCardNumber)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "244000000",
                WalletType = WalletType.CARD,
                AccountScheme = AccountScheme.MASTERCARD,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(false, InvalidReasons.InvalidCardNumber)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "234567",
                WalletType = WalletType.CARD,
                AccountScheme = AccountScheme.MTN,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(false, InvalidReasons.InvalidAccountSchemeForCardWallets)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "234567",
                WalletType = WalletType.CARD,
                AccountScheme = AccountScheme.AIRTELTIGO,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(false, InvalidReasons.InvalidAccountSchemeForCardWallets)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "234567",
                WalletType = WalletType.CARD,
                AccountScheme = AccountScheme.VODAFONE,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(false, InvalidReasons.InvalidAccountSchemeForCardWallets)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "724445",
                WalletType = WalletType.CARD,
                AccountScheme = AccountScheme.MASTERCARD,
                OwnerPhoneNumber = "+233244000000"
            },
            new ValidationResult(false, InvalidReasons.InvalidWalletOwnerPhoneNumberFormat)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "0244000000",
                WalletType = WalletType.CARD,
                AccountScheme = AccountScheme.MASTERCARD,
                OwnerPhoneNumber = "+233 (0) 244000000"
            },
            new ValidationResult(false, InvalidReasons.InvalidWalletOwnerPhoneNumberFormat)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "123456",
                WalletType = WalletType.CARD,
                AccountScheme = AccountScheme.MASTERCARD,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(true)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "123456",
                WalletType = WalletType.CARD,
                AccountScheme = AccountScheme.VISA,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(true)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "0266123456",
                WalletType = WalletType.MOMO,
                AccountScheme = AccountScheme.AIRTELTIGO,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(true)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "0244123456",
                WalletType = WalletType.MOMO,
                AccountScheme = AccountScheme.MTN,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(true)
        },
        {
            new WalletWriteDto
            {
                WalletName = "Frodo Baggins",
                AccountNumber = "0200123456",
                WalletType = WalletType.MOMO,
                AccountScheme = AccountScheme.VODAFONE,
                OwnerPhoneNumber = "233244000000"
            },
            new ValidationResult(true)
        },

    };
}
