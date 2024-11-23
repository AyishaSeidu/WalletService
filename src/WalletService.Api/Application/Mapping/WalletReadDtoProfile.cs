using AutoMapper;
using WalletService.Api.Application.Dtos;
using WalletService.Domain.Models;

namespace WalletService.Api.Application.Mapping;

public class WalletReadDtoProfile : Profile
{
    public WalletReadDtoProfile()
    {
        CreateMap<Wallet, WalletReadDto>();
    }
}
