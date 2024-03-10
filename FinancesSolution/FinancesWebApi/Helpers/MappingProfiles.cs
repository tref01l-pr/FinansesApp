using AutoMapper;
using FinancesWebApi.Dto;
using FinancesWebApi.Models;
using FinancesWebApi.Models.User;
using FinancesWebApi.Models.User.UserSettings;

namespace FinancesWebApi.Helpers;

public class MappingProfiles : Profile
{
    public MappingProfiles()
    {
        CreateMap<User, UserDto>();
        CreateMap<RegisterDto, User>();

        CreateMap<CountryPhoneNumber, NumberWithMaskDto>();
        CreateMap<NumberDto, UserPhoneNumber>();
    }
}