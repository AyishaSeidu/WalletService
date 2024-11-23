using System.ComponentModel.DataAnnotations;
using WalletService.Api.Application.Dtos.Enums;

namespace WalletService.Api.Application.Dtos;

public class WalletWriteDto
{
    public required string WalletName { get; set; }
    public required string AccountNumber { get; set; }
    public WalletType WalletType {  get; set; }
    public AccountScheme AccountScheme {  get; set; }

    [RegularExpression(@"^0\d{9}$", ErrorMessage = "The number must be 10 digits, and must start with 0")]
    public required string OwnerOwnerPhoneNumber {  get; set; }
}
