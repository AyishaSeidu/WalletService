namespace WalletService.Api.Application.Validation;

public interface IValidationResult
{
    bool IsValid { get; }
    string? InvalidReason { get; }
}
