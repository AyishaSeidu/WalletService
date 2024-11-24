namespace WalletService.Api.Application.Validation;

public class ValidationResult(bool isValid, string? invalidReason = null) : IValidationResult
{
    private readonly bool _isValid = isValid;
    private readonly string? _invalidReason = invalidReason;

    public bool IsValid => _isValid;

    public string? InvalidReason => _invalidReason;
}
