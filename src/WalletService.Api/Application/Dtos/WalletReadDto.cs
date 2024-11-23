using WalletService.Api.Application.Dtos.Enums;

namespace WalletService.Api.Application.Dtos;

public class WalletReadDto
{
    public int Id { get; set; }
    public required string WalletName { get; set; }
    public required string AccountNumber { get; set; }
    public WalletType WalletType { get; set; }
    public AccountScheme AccountScheme { get; set; }
    public required string OwnerPhoneNumber { get; set; }
}
