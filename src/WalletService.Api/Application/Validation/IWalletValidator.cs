using WalletService.Api.Application.Dtos;

namespace WalletService.Api.Application.Validation;

public interface IWalletValidator
{
    IValidationResult ValidateWallet(WalletWriteDto wallet);
}
