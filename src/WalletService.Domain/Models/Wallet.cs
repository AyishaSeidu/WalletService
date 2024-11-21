
using System.ComponentModel.DataAnnotations;
using WalletService.Domain.Models.Enums;

namespace WalletService.Domain.Models;

public class Wallet
{
    private readonly string _walletName;
    private readonly WalletType _walletType;
    private readonly AccountScheme _accountScheme;
    private readonly string _walletOwner;
    private readonly string _accountNumber;
    private readonly DateTime _createdAt;
    private  bool _isActive;
    private  DateTime? _updatedAt;

    public Wallet(string walletName, string accountNumber, WalletType walletType, AccountScheme accountScheme, string owner)
    {
        _walletName = walletName;
        _walletType = walletType;
        _accountScheme = accountScheme;
        _walletOwner = owner;
        _accountNumber = accountNumber;
        _createdAt = DateTime.UtcNow;
        _isActive = true;
    }
    [Key]
    public int WalletId { get; set; }
    public string WalletName => _walletName;
    public string AccountNumber => _accountNumber;
    public WalletType WalletType => _walletType;
    public AccountScheme AccountScheme => _accountScheme;
    public DateTime CreatedAt => _createdAt;
    public string Owner => _walletOwner;
    public bool IsActive => _isActive;
    public DateTime? UpdatedAt => _updatedAt;


    public void DeactivateWallet()
    {
        _isActive = false;
        _updatedAt = DateTime.UtcNow;
    }

    //EF core needs an empty contructor to read data into
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    protected Wallet()
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider adding the 'required' modifier or declaring as nullable.
    {
        
    }
}
