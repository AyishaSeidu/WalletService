
namespace WalletService.Api.Application.Validation;

public static class InvalidReasons
{
    public const string NullWallet = "Invalid wallet: wallet cannot be null";

    public const string InvalidWalletName = "Invalid Wallet name, please provide a valid string";

    public const string EmptyWalletOwnerNumber = "Invalid phone number: phone number cannot be empty";

    public const string EmptyAccountNumber = "Invalid account number: Account number cannot be empty";

    public const string UnknownWalletType = "Unsupported wallet type: Wallet type must be MoMo or Card";

    public const string UnknownAccountScheme = "Unsupported account scheme selected: Please choose a valid option";

    public const string InvalidWalletOwnerPhoneNumberFormat = "Invalid wallet owner's mobile number: Please enter a valid international number without spaces or the '+' symbol (e.g., 233247000000).";

    public const string InvalidAccountSchemeForMoMoWallets = "Invalid Account Scheme selected for MoMo wallet. MoMo wallets can only be Mtn, Vodafone or AirtelTigo";

    public const string InvalidAccountSchemeForCardWallets = "Invalid Account Scheme selected for Card wallet. Card wallets can only be MasterCard or Visa";

    public const string InvalidMoMoNumber = "Invalid MoMo number: A MoMo number must be 10 digits starting with 0";

    public const string InvalidCardNumber = "Invalid Card number: A Card number must be the first 6 digits on your card";
}
