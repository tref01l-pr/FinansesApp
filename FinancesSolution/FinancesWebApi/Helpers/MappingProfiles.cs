using AutoMapper;
using FinancesWebApi.Dto;
using FinancesWebApi.Models;

namespace FinancesWebApi.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<User, UserDto>();
        CreateMap<RegisterDto, User>();
    }
}